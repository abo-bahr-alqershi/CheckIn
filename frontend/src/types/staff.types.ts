
// DTO for staff data
export interface StaffDto {
  id: string;
  userId: string;
  userName: string;
  propertyId: string;
  propertyName: string;
  position: StaffPosition;
  permissions: string;
}

export type StaffPosition = {
  Manager: 0,
  Receptionist: 1,
  Housekeeping: 2,
  Maintenance: 3,
  Other: 4,
}

// Command to add a new staff member
export interface AddStaffCommand {
  userId: string;
  propertyId: string;
  position: string;
  permissions: string;
}

// Command to update staff member details
export interface UpdateStaffCommand {
  staffId: string;
  position?: string;
  permissions?: string;
}

// Command to remove a staff member
export interface RemoveStaffCommand {
  staffId: string;
}

// Query to get staff by position
export interface GetStaffByPositionQuery {
  position: string;
  propertyId?: string;
  pageNumber?: number;
  pageSize?: number;
}

// Query to get staff by property
export interface GetStaffByPropertyQuery {
  propertyId: string;
  pageNumber?: number;
  pageSize?: number;
}

// Query to get staff details by user
export interface GetStaffByUserQuery {
  userId: string;
} 