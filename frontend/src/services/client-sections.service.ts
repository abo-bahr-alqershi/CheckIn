import { apiClient } from './api.service';
import type { PagedResultDto } from '../types/common.types';

const BASE = '/api/client/sections';

class ClientSectionsService {
	async getSectionItems(sectionId: string, params: { pageNumber: number; pageSize: number }): Promise<PagedResultDto<any>> {
		const { data } = await apiClient.get(`${BASE}/${sectionId}/items`, { params });
		return data;
	}
}

export default new ClientSectionsService();