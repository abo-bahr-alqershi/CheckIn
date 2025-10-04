import React, { useState } from 'react';
// أيقونات بسيطة بدلاً من lucide-react
const SearchIcon = () => <span>🔍</span>;
const UserIcon = () => <span>👤</span>;
const FilterIcon = () => <span>🔽</span>;
const EyeIcon = () => <span>👁️</span>;
const ClockIcon = () => <span>🕒</span>;
const DatabaseIcon = () => <span>💾</span>;
import { useAuditLogs } from '../../hooks/useAdminAuditLogs';
import type { AuditLogDto, AuditLogsQuery } from '../../types/audit-log.types';
import DataTable from '../../components/common/DataTable';
import Modal from '../../components/common/Modal';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import UserSelector from '../../components/selectors/UserSelector';

const AdminAuditLogs = () => {
  const [query, setQuery] = useState<AuditLogsQuery>({
    pageNumber: 1,
    pageSize: 20,
  });
  const [selectedLog, setSelectedLog] = useState<AuditLogDto | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [filterValues, setFilterValues] = useState({ userId: '', from: '', to: '', operationType: '' });
  const [showAdvancedFilters, setShowAdvancedFilters] = useState<boolean>(false);

  // Define filter options for SearchAndFilter
  const filterOptions: FilterOption[] = [
    {
      key: 'userId',
      label: 'المستخدم',
      type: 'custom',
      render: (value, onChange) => (
        <UserSelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر المستخدم"
          className="w-full"
        />
      ),
    },
    { key: 'from', label: 'من تاريخ', type: 'date' },
    { key: 'to', label: 'إلى تاريخ', type: 'date' },
    { key: 'operationType', label: 'نوع العملية', type: 'text', placeholder: 'نوع العملية' },
  ];

  const { data, isLoading, error } = useAuditLogs(query);

  // Manual filter handlers removed in favor of SearchAndFilter

  const handleViewDetails = (log: AuditLogDto) => {
    setSelectedLog(log);
    setIsModalOpen(true);
  };

  const columns = [
    {
      header: 'التوقيت',
      title: 'التوقيت',
      key: 'timestamp',
      render: (_value: any, log: AuditLogDto) => (
        <div className="flex items-center gap-2">
          <ClockIcon />
          <div>
            <div className="text-sm font-medium">
              {new Date(log.timestamp).toLocaleDateString('ar-SA')}
            </div>
            <div className="text-xs text-gray-500">
              {new Date(log.timestamp).toLocaleTimeString('ar-SA')}
            </div>
          </div>
        </div>
      ),
    },
    {
      header: 'المستخدم',
      title: 'المستخدم',
      key: 'username',
      render: (_value: any, log: AuditLogDto) => (
        <div className="flex items-center gap-2">
          <UserIcon />
          <div>
            <div className="text-sm font-medium">{log.username}</div>
            <div className="text-xs text-gray-500">{log.userId}</div>
          </div>
        </div>
      ),
    },
    {
      header: 'العملية',
      title: 'العملية',
      key: 'action',
      render: (_value: any, log: AuditLogDto) => (
        <div className="flex items-center gap-2">
          <DatabaseIcon />
          <div>
            <div className="text-sm font-medium">{log.action}</div>
            <div className="text-xs text-gray-500">{log.tableName}</div>
          </div>
        </div>
      ),
    },
    {
      header: 'السجل المتأثر',
      title: 'السجل المتأثر',
      key: 'recordName',
      render: (_value: any, log: AuditLogDto) => (
        <div>
          <div className="text-sm font-medium">{log.recordName}</div>
          <div className="text-xs text-gray-500">{log.recordId}</div>
        </div>
      ),
    },
    {
      header: 'التغييرات',
      title: 'التغييرات',
      key: 'changes',
      render: (_value: any, log: AuditLogDto) => (
        <div className="max-w-xs">
          <div className="text-sm text-gray-700 truncate">{log.changes}</div>
          {log.isSlowOperation && (
            <div className="text-xs text-orange-600 bg-orange-50 px-2 py-1 rounded mt-1 inline-block">
              عملية بطيئة
            </div>
          )}
        </div>
      ),
    },
    {
      header: 'الإجراءات',
      title: 'الإجراءات',
      key: 'actions',
      render: (_value: any, log: AuditLogDto) => (
        <button
          onClick={() => handleViewDetails(log)}
          className="flex items-center gap-1 px-3 py-1 bg-blue-50 text-blue-600 rounded hover:bg-blue-100 transition-colors"
        >
          <EyeIcon />
          عرض التفاصيل
        </button>
      ),
    },
  ];

  const ChangeViewer = ({ log }: { log: AuditLogDto }) => {
    if (!log.oldValues && !log.newValues) {
      return (
        <div className="text-gray-500 text-center py-4">
          لا توجد تفاصيل التغييرات متاحة
        </div>
      );
    }

    const oldValues = log.oldValues || {};
    const newValues = log.newValues || {};
    const allKeys = new Set([...Object.keys(oldValues), ...Object.keys(newValues)]);

    return (
      <div className="space-y-4">
        {Array.from(allKeys).map(key => {
          const oldValue = oldValues[key];
          const newValue = newValues[key];
          const hasChanged = oldValue !== newValue;

          return (
            <div key={key} className="border rounded-lg p-4">
              <div className="font-medium text-gray-700 mb-2">{key}</div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <div className="text-sm text-gray-500 mb-1">القيمة القديمة</div>
                  <div className={`p-2 rounded border ${hasChanged ? 'bg-red-50 border-red-200' : 'bg-gray-50'}`}>
                    {oldValue !== undefined ? String(oldValue) : 'غير محدد'}
                  </div>
                </div>
                <div>
                  <div className="text-sm text-gray-500 mb-1">القيمة الجديدة</div>
                  <div className={`p-2 rounded border ${hasChanged ? 'bg-green-50 border-green-200' : 'bg-gray-50'}`}>
                    {newValue !== undefined ? String(newValue) : 'غير محدد'}
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>
    );
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">سجلات التدقيق</h1>
          <p className="text-gray-600">مراقبة جميع الأنشطة والعمليات في النظام</p>
        </div>
      </div>

      <SearchAndFilter
        searchPlaceholder="البحث في سجلات التدقيق..."
        searchValue={searchTerm}
        onSearchChange={(value) => {
          setSearchTerm(value);
          setQuery((prev) => ({ ...prev, pageNumber: 1, searchTerm: value }));
        }}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={(key, value) => {
          setFilterValues((prev) => ({ ...prev, [key]: value }));
          setQuery((prev) => ({ ...prev, pageNumber: 1, [key]: value }));
        }}
        onReset={() => {
          setSearchTerm('');
          setFilterValues({ userId: '', from: '', to: '', operationType: '' });
          setQuery((prev) => ({
            ...prev,
            pageNumber: 1,
            searchTerm: undefined,
            userId: undefined,
            from: undefined,
            to: undefined,
            operationType: undefined,
          }));
        }}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters((prev) => !prev)}
      />

      {/* الجدول */}
      <div className="bg-white rounded-lg shadow">
        <DataTable
          data={data?.items || []}
          columns={columns}
          loading={isLoading}
          pagination={{
            current: data?.pageNumber ?? query.pageNumber ?? 1,
            total: data?.totalCount ?? 0,
            pageSize: data?.pageSize ?? query.pageSize ?? 20,
            onChange: (page, size) => setQuery(prev => ({ ...prev, pageNumber: page, pageSize: size })),
          }}
          onRowClick={() => {}}
        />
      </div>

      {/* مودال تفاصيل السجل */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="تفاصيل سجل التدقيق"
        size="lg"
      >
        {selectedLog && (
          <div className="space-y-6">
            {/* معلومات أساسية */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
              <div>
                <div className="text-sm text-gray-500">التوقيت</div>
                <div className="font-medium">
                  {new Date(selectedLog.timestamp).toLocaleString('ar-SA')}
                </div>
              </div>
              <div>
                <div className="text-sm text-gray-500">المستخدم</div>
                <div className="font-medium">{selectedLog.username}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">العملية</div>
                <div className="font-medium">{selectedLog.action}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">الجدول</div>
                <div className="font-medium">{selectedLog.tableName}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">معرف السجل</div>
                <div className="font-medium">{selectedLog.recordId}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">اسم السجل</div>
                <div className="font-medium">{selectedLog.recordName}</div>
              </div>
            </div>

            {/* الملاحظات */}
            {selectedLog.notes && (
              <div>
                <div className="text-sm text-gray-500 mb-2">الملاحظات</div>
                <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg">
                  {selectedLog.notes}
                </div>
              </div>
            )}

            {/* التغييرات */}
            <div>
              <div className="text-sm text-gray-500 mb-2">وصف التغييرات</div>
              <div className="p-3 bg-gray-50 border rounded-lg">
                {selectedLog.changes}
              </div>
            </div>

            {/* تفاصيل التغييرات */}
            <div>
              <div className="text-lg font-medium mb-4">تفاصيل التغييرات</div>
              <ChangeViewer log={selectedLog} />
            </div>

            {/* البيانات الإضافية */}
            {selectedLog.metadata && Object.keys(selectedLog.metadata).length > 0 && (
              <div>
                <div className="text-sm text-gray-500 mb-2">البيانات الإضافية</div>
                <div className="p-3 bg-gray-50 border rounded-lg">
                  <pre className="text-sm overflow-x-auto">
                    {JSON.stringify(selectedLog.metadata, null, 2)}
                  </pre>
                </div>
              </div>
            )}
          </div>
        )}
      </Modal>
    </div>
  );
};

export default AdminAuditLogs;