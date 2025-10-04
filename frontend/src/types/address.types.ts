export interface AddressDto {
  street: string;
  city: string;
  state?: string;
  country: string;
  postalCode?: string;
  latitude?: number;
  longitude?: number;
  fullAddress: string;
  hasCoordinates: boolean;
}