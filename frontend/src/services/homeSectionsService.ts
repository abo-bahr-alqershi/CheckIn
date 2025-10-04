// frontend/src/services/homeSectionsService.ts
import { apiClient } from './api.service';

// Base URLs for Home Sections admin and client endpoints
const ADMIN_BASE = '/api/admin/home-sections';
const CLIENT_BASE = '/api/client/home-sections';

class HomeSectionsService {
  async getCityDestinations(params?: { onlyActive?: boolean; limit?: number }): Promise<any[]> {
    const response = await apiClient.get(`${ADMIN_BASE}/city-destinations`, { params });
    return response.data;
  }
}

export default new HomeSectionsService();