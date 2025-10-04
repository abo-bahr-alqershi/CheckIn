import { useEffect, useMemo, useState } from 'react';
import ModernDashboardLayout from '../../components/layout/ModernDashboardLayout';
import DataTable, { type Column } from '../../components/common/DataTable';
import { SectionTarget, SectionType } from '../../types/enums';
import type { SectionDto, CreateSectionCommand, UpdateSectionCommand, GetSectionsQuery } from '../../types/sections.types';
import AdminSectionsService from '../../services/admin-sections.service';
import Modal from '../../components/common/Modal';

const AdminSections = () => {
	const [items, setItems] = useState<SectionDto[]>([]);
	const [totalCount, setTotalCount] = useState(0);
	const [pageNumber, setPageNumber] = useState(1);
	const [pageSize, setPageSize] = useState(20);
	const [filters, setFilters] = useState<{ target?: SectionTarget; type?: SectionType }>({});
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);

	const [showModal, setShowModal] = useState(false);
	const [editing, setEditing] = useState<SectionDto | null>(null);
	const [form, setForm] = useState<CreateSectionCommand>({ type: SectionType.HORIZONTAL_PROPERTY_LIST, displayOrder: 0, target: SectionTarget.PROPERTIES, isActive: true });

	const columns: Column<SectionDto>[] = useMemo(() => [
		{ key: 'displayOrder', title: 'الترتيب', width: '80px' },
		{ key: 'type', title: 'النوع', render: (_, row) => row.type },
		{ key: 'target', title: 'المستهدف', render: (_, row) => row.target === SectionTarget.PROPERTIES ? 'كيانات' : 'وحدات' },
		{ key: 'isActive', title: 'نشط', render: (_, row) => row.isActive ? 'نعم' : 'لا', width: '80px' },
	], []);

	const load = async () => {
		try {
			setLoading(true);
			const query: GetSectionsQuery = { pageNumber, pageSize, target: filters.target, type: filters.type };
			const res = await AdminSectionsService.getSections(query);
			setItems(res.items as any);
			setTotalCount(res.totalCount);
		} catch (e: any) {
			setError(e.message || 'حدث خطأ');
		} finally { setLoading(false); }
	};

	useEffect(() => { void load(); }, [pageNumber, pageSize, filters]);

	const resetForm = () => {
		setForm({ type: SectionType.HORIZONTAL_PROPERTY_LIST, displayOrder: 0, target: SectionTarget.PROPERTIES, isActive: true });
		setEditing(null);
	};

	const onCreate = async () => {
		await AdminSectionsService.createSection(form);
		setShowModal(false);
		resetForm();
		void load();
	};

	const onUpdate = async () => {
		if (!editing) return;
		const payload: UpdateSectionCommand = { sectionId: editing.id, type: form.type, displayOrder: form.displayOrder, target: form.target, isActive: (form as any).isActive ?? true };
		await AdminSectionsService.updateSection(editing.id, payload);
		setShowModal(false);
		resetForm();
		void load();
	};

	const onDelete = async (row: SectionDto) => {
		await AdminSectionsService.deleteSection(row.id);
		void load();
	};

	return (
		<ModernDashboardLayout>
			<div className="flex items-center gap-4 mb-4">
				<div className="flex items-center gap-2">
					<label className="text-sm">المستهدف</label>
					<select className="border rounded px-2 py-1" value={filters.target || ''} onChange={(e) => setFilters({ ...filters, target: (e.target.value || undefined) as any })}>
						<option value="">الكل</option>
						<option value={SectionTarget.PROPERTIES}>كيانات</option>
						<option value={SectionTarget.UNITS}>وحدات</option>
					</select>
				</div>
				<div className="flex items-center gap-2">
					<label className="text-sm">النوع</label>
					<select className="border rounded px-2 py-1" value={filters.type || ''} onChange={(e) => setFilters({ ...filters, type: (e.target.value || undefined) as any })}>
						<option value="">الكل</option>
						{Object.values(SectionType).map(v => (
							<option key={v} value={v}>{v}</option>
						))}
					</select>
				</div>
				<button className="btn btn-primary ml-auto" onClick={() => { resetForm(); setShowModal(true); }}>إنشاء قسم</button>
			</div>
			<DataTable
				data={items}
				columns={columns}
				loading={loading}
				pagination={{ current: pageNumber, pageSize, total: totalCount, onChange: (p, s) => { setPageNumber(p); setPageSize(s); } }}
				actions={[
					{ label: 'تعديل', onClick: (row) => { setEditing(row); setForm({ type: row.type, displayOrder: row.displayOrder, target: row.target, isActive: row.isActive }); setShowModal(true); }, color: 'blue' },
					{ label: 'حذف', onClick: onDelete, color: 'red' },
				]}
			/>
			<Modal isOpen={showModal} onClose={() => setShowModal(false)} title={editing ? 'تعديل قسم' : 'إنشاء قسم'}>
				<div className="space-y-4">
					<div className="flex items-center gap-2">
						<label className="text-sm w-28">النوع</label>
						<select className="border rounded px-2 py-1 flex-1" value={form.type} onChange={(e) => setForm({ ...form, type: e.target.value as any })}>
							{Object.values(SectionType).map(v => (<option key={v} value={v}>{v}</option>))}
						</select>
					</div>
					<div className="flex items-center gap-2">
						<label className="text-sm w-28">المستهدف</label>
						<select className="border rounded px-2 py-1 flex-1" value={form.target} onChange={(e) => setForm({ ...form, target: e.target.value as any })}>
							<option value={SectionTarget.PROPERTIES}>كيانات</option>
							<option value={SectionTarget.UNITS}>وحدات</option>
						</select>
					</div>
					<div className="flex items-center gap-2">
						<label className="text-sm w-28">الترتيب</label>
						<input className="border rounded px-2 py-1 flex-1" type="number" value={form.displayOrder} onChange={(e) => setForm({ ...form, displayOrder: Number(e.target.value) })} />
					</div>
					<div className="flex justify-end gap-2">
						<button className="btn" onClick={() => setShowModal(false)}>إلغاء</button>
						<button className="btn btn-primary" onClick={editing ? onUpdate : onCreate}>{editing ? 'حفظ' : 'إنشاء'}</button>
					</div>
				</div>
			</Modal>
		</ModernDashboardLayout>
	);
};

export default AdminSections;