// أنواع بيانات مجموعات الحقول (Field Group Types)

import type { UnitTypeFieldDto } from "./unit-type-field.types";

/**
 * بيانات مجموعة حقول
 */
export interface FieldGroupDto {
  groupId: string;
  unitTypeId: string;
  groupName: string;
  displayName: string;
  description: string;
  sortOrder: number;
  isCollapsible: boolean;
  isExpandedByDefault: boolean;
  fields: UnitTypeFieldDto[];
}

export interface CreateFieldGroupCommand {
  unitTypeId: string;
  groupName: string;
  displayName?: string;
  description?: string;
  sortOrder: number;
  isCollapsible?: boolean;
  isExpandedByDefault?: boolean;
}

export interface UpdateFieldGroupCommand {
  groupId: string;
  groupName?: string;
  displayName?: string;
  description?: string;
  sortOrder?: number;
  isCollapsible?: boolean;
  isExpandedByDefault?: boolean;
}

export interface DeleteFieldGroupCommand {
  groupId: string;
}

export interface GroupOrderDto {
  groupId: string;
  sortOrder: number;
}

export interface ReorderFieldGroupsCommand {
  propertyTypeId: string;
  groupOrders: GroupOrderDto[];
}

export interface GetFieldGroupByIdQuery {
  groupId: string;
}

export interface GetFieldGroupsByUnitTypeQuery {
  unitTypeId: string;
}

export interface AssignFieldToGroupCommand {
  groupId: string;
  fieldId: string;
}

export interface GroupOrderDto {
  groupId: string;
  sortOrder: number;
} 