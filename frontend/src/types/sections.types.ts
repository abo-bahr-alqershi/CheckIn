import type { SectionType, SectionTarget } from './enums';

export interface SectionDto {
	id: string;
	type: SectionType;
	displayOrder: number;
	target: SectionTarget;
	isActive: boolean;
	items: SectionItemDto[];
}

export interface SectionItemDto {
	id: string;
	sectionId: string;
	propertyId?: string | null;
	unitId?: string | null;
	sortOrder: number;
}

export interface GetSectionsQuery {
	pageNumber: number;
	pageSize: number;
	target?: SectionTarget;
	type?: SectionType;
}

export interface CreateSectionCommand {
	type: SectionType;
	displayOrder: number;
	target: SectionTarget;
	isActive?: boolean;
}

export interface UpdateSectionCommand {
	sectionId: string;
	type: SectionType;
	displayOrder: number;
	target: SectionTarget;
	isActive: boolean;
}

export interface AssignSectionItemsCommand {
	propertyIds?: string[];
	unitIds?: string[];
}