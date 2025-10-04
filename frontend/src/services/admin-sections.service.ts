import { apiClient } from './api.service';
import type { PagedResultDto } from '../types/common.types';
import type { SectionDto, CreateSectionCommand, UpdateSectionCommand, AssignSectionItemsCommand, GetSectionsQuery } from '../types/sections.types';
import { SectionType, SectionTarget } from '../types/enums';

const BASE = '/api/admin/sections';

const mapSectionTypeToBackend = (t: SectionType): string => {
	switch (t) {
		case SectionType.SINGLE_PROPERTY_AD: return 'SinglePropertyAd';
		case SectionType.MULTI_PROPERTY_AD: return 'MultiPropertyAd';
		case SectionType.UNIT_SHOWCASE_AD: return 'UnitShowcaseAd';
		case SectionType.SINGLE_PROPERTY_OFFER: return 'SinglePropertyOffer';
		case SectionType.LIMITED_TIME_OFFER: return 'LimitedTimeOffer';
		case SectionType.SEASONAL_OFFER: return 'SeasonalOffer';
		case SectionType.MULTI_PROPERTY_OFFERS_GRID: return 'MultiPropertyOffersGrid';
		case SectionType.OFFERS_CAROUSEL: return 'OffersCarousel';
		case SectionType.FLASH_DEALS: return 'FlashDeals';
		case SectionType.HORIZONTAL_PROPERTY_LIST: return 'HorizontalPropertyList';
		case SectionType.VERTICAL_PROPERTY_GRID: return 'VerticalPropertyGrid';
		case SectionType.MIXED_LAYOUT_LIST: return 'MixedLayoutList';
		case SectionType.COMPACT_PROPERTY_LIST: return 'CompactPropertyList';
		case SectionType.CITY_CARDS_GRID: return 'CityCardsGrid';
		case SectionType.DESTINATION_CAROUSEL: return 'DestinationCarousel';
		case SectionType.EXPLORE_CITIES: return 'ExploreCities';
		case SectionType.PREMIUM_CAROUSEL: return 'PremiumCarousel';
		case SectionType.INTERACTIVE_SHOWCASE: return 'InteractiveShowcase';
		default: return 'HorizontalPropertyList';
	}
};

const mapSectionTargetToBackend = (t: SectionTarget): string => {
	return t === SectionTarget.PROPERTIES ? 'Properties' : 'Units';
};

class AdminSectionsService {
	async getSections(params: GetSectionsQuery): Promise<PagedResultDto<SectionDto>> {
		const { data } = await apiClient.get(BASE, { params });
		return data;
	}

	async createSection(payload: CreateSectionCommand): Promise<SectionDto> {
		const body = {
			...payload,
			type: mapSectionTypeToBackend(payload.type),
			target: mapSectionTargetToBackend(payload.target),
		};
		const { data } = await apiClient.post(BASE, body);
		return data.data;
	}

	async updateSection(sectionId: string, payload: UpdateSectionCommand): Promise<SectionDto> {
		const body = {
			...payload,
			type: mapSectionTypeToBackend(payload.type),
			target: mapSectionTargetToBackend(payload.target),
		};
		const { data } = await apiClient.put(`${BASE}/${sectionId}`, body);
		return data.data;
	}

	async deleteSection(sectionId: string): Promise<void> {
		await apiClient.delete(`${BASE}/${sectionId}`);
	}

	async assignItems(sectionId: string, payload: AssignSectionItemsCommand): Promise<void> {
		await apiClient.post(`${BASE}/${sectionId}/assign-items`, payload);
	}
}

export default new AdminSectionsService();