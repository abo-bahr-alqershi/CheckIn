// frontend/src/components/admin/ContentManagementPanel.tsx

import React, { useEffect, useMemo, useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Stack,
  TextField,
  Button,
  Grid,
  Card,
  CardContent,
  CardMedia,
  IconButton,
  Chip,
  Switch,
  Divider,
} from '@mui/material';
import { ArrowUpward, ArrowDownward, Delete as DeleteIcon, Add as AddIcon, Refresh as RefreshIcon } from '@mui/icons-material';
import type { DynamicContent } from '../../types/homeSections.types';
import { AdminPropertiesService } from '../../services/admin-properties.service';
import HomeSectionsService from '../../services/homeSectionsService';

interface ContentManagementPanelProps {
  sectionType: string;
  currentContent: DynamicContent[];
  onContentChange: (content: DynamicContent[]) => void;
  maxItems?: number;
}

const simplifyProperty = (p: any) => ({
  id: p.id,
  name: p.name,
  city: p.city,
  mainImageUrl: p.images?.[0]?.url || p.mainImageUrl,
  basePrice: p.basePrice?.amount ?? p.basePrice,
  currency: p.basePrice?.currency ?? p.currency,
});

const simplifyCity = (c: any) => ({
  id: c.id,
  name: c.name,
  nameAr: c.nameAr,
  country: c.country,
  countryAr: c.countryAr,
  imageUrl: c.imageUrl,
});

const simplifyAd = (a: any) => ({
  id: a.id,
  title: a.title,
  subtitle: a.subtitle,
  description: a.description,
  customImageUrl: a.customImageUrl,
  ctaText: a.ctaText,
});

const getAllowedTypes = (sectionType: string): ('PROPERTY' | 'DESTINATION' | 'ADVERTISEMENT')[] => {
  const t = sectionType || '';
  if (/(CITY|DESTINATION)/.test(t)) return ['DESTINATION'];
  if (/(AD|OFFER)/.test(t)) return ['PROPERTY', 'ADVERTISEMENT'];
  if (/(PROPERTY|LIST|GRID|SHOWCASE)/.test(t)) return ['PROPERTY'];
  return ['PROPERTY', 'DESTINATION', 'ADVERTISEMENT'];
};

const ContentManagementPanel: React.FC<ContentManagementPanelProps> = ({ sectionType, currentContent, onContentChange, maxItems = 10 }) => {
  const [selectedTab, setSelectedTab] = useState<'PROPERTY' | 'DESTINATION' | 'ADVERTISEMENT'>(getAllowedTypes(sectionType)[0]);
  const [query, setQuery] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [items, setItems] = useState<any[]>([]);
  const [content, setContent] = useState<DynamicContent[]>(currentContent || []);

  useEffect(() => setContent(currentContent || []), [currentContent]);

  const allowed = useMemo(() => getAllowedTypes(sectionType), [sectionType]);

  const fetchItems = async () => {
    setIsLoading(true);
    try {
      if (selectedTab === 'PROPERTY') {
        const res = await AdminPropertiesService.getAll({ pageNumber: 1, pageSize: 50 });
        const list = (res.items || []).map(simplifyProperty);
        setItems(list);
      } else if (selectedTab === 'DESTINATION') {
        const res = await HomeSectionsService.getCityDestinations({ onlyActive: true, limit: 50 });
        setItems((res || []).map(simplifyCity));
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchItems(); }, [selectedTab]);

  const filteredItems = useMemo(() => {
    if (!query) return items;
    const q = query.toLowerCase();
    return items.filter((it) => JSON.stringify(it).toLowerCase().includes(q));
  }, [items, query]);

  const canAddMore = content.length < maxItems;

  const addItem = (data: any) => {
    if (!canAddMore) return;
    const contentType = selectedTab;
    const dc: DynamicContent = {
      id: `tmp-${Date.now()}`,
      sectionId: '',
      contentType,
      contentData: data,
      metadata: {},
      displayOrder: content.length,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    } as any;
    const updated = [...content, dc];
    setContent(updated);
    onContentChange(updated);
  };

  const removeItem = (id: string) => {
    const updated = content.filter((c) => c.id !== id).map((c, i) => ({ ...c, displayOrder: i }));
    setContent(updated);
    onContentChange(updated);
  };

  const toggleActive = (id: string) => {
    const updated = content.map((c) => (c.id === id ? { ...c, isActive: !c.isActive } : c));
    setContent(updated);
    onContentChange(updated);
  };

  const move = (index: number, dir: -1 | 1) => {
    const target = index + dir;
    if (target < 0 || target >= content.length) return;
    const arr = [...content];
    const tmp = arr[index];
    arr[index] = arr[target];
    arr[target] = tmp;
    const updated = arr.map((c, i) => ({ ...c, displayOrder: i }));
    setContent(updated);
    onContentChange(updated);
  };

  const clearAll = () => {
    setContent([]);
    onContentChange([]);
  };

  return (
    <Box>
      <Paper elevation={0} sx={{ p: 2, mb: 2 }}>
        <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={2}>
          <Stack direction="row" spacing={1} alignItems="center">
            {allowed.map((t) => (
              <Button
                key={t}
                variant={selectedTab === t ? 'contained' : 'outlined'}
                size="small"
                onClick={() => setSelectedTab(t)}
              >
                {t === 'PROPERTY' ? 'عقارات' : t === 'DESTINATION' ? 'وجهات' : 'إعلانات'}
              </Button>
            ))}
          </Stack>
          <Stack direction="row" spacing={1} alignItems="center">
            <TextField
              size="small"
              placeholder="بحث..."
              value={query}
              onChange={(e) => setQuery(e.target.value)}
            />
            <Button variant="outlined" size="small" startIcon={<RefreshIcon />} onClick={fetchItems} disabled={isLoading}>
              تحديث
            </Button>
            <Divider flexItem orientation="vertical" />
            <Chip label={`العناصر المحددة: ${content.length}/${maxItems}`} color={content.length >= maxItems ? 'error' : 'primary'} />
            <Button variant="text" color="error" size="small" onClick={clearAll} disabled={content.length === 0}>
              حذف الكل
            </Button>
          </Stack>
        </Stack>
      </Paper>

      {/* Selected Content */}
      {content.length > 0 && (
        <Paper elevation={0} sx={{ p: 2, mb: 2, border: '1px solid', borderColor: 'divider' }}>
          <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 1 }}>
            المحتوى الحالي
          </Typography>
          <Grid container spacing={2}>
            {content
              .sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0))
              .map((c, index) => (
                <Grid key={c.id} item xs={12} md={6} lg={4}>
                  <Card>
                    <CardMedia
                      component="img"
                      height={140}
                      image={c.contentData?.mainImageUrl || c.contentData?.imageUrl || c.contentData?.customImageUrl}
                      alt={c.contentData?.name || c.contentData?.title}
                    />
                    <CardContent sx={{ py: 1.5 }}>
                      <Stack direction="row" alignItems="center" justifyContent="space-between">
                        <Stack>
                          <Typography variant="subtitle2" noWrap>
                            {c.contentData?.nameAr || c.contentData?.name || c.contentData?.title || c.contentType}
                          </Typography>
                          <Chip size="small" label={c.contentType} sx={{ mt: 0.5, width: 'fit-content' }} />
                        </Stack>
                        <Stack direction="row" spacing={1} alignItems="center">
                          <IconButton size="small" onClick={() => move(index, -1)} disabled={index === 0}>
                            <ArrowUpward fontSize="small" />
                          </IconButton>
                          <IconButton size="small" onClick={() => move(index, 1)} disabled={index === content.length - 1}>
                            <ArrowDownward fontSize="small" />
                          </IconButton>
                          <Switch size="small" checked={!!c.isActive} onChange={() => toggleActive(c.id)} />
                          <IconButton size="small" color="error" onClick={() => removeItem(c.id)}>
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Stack>
                      </Stack>
                    </CardContent>
                  </Card>
                </Grid>
              ))}
          </Grid>
        </Paper>
      )}

      {/* Available Items */}
      <Paper elevation={0} sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
        <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ mb: 2 }}>
          <Typography variant="subtitle1" fontWeight={600}>
            {selectedTab === 'PROPERTY' ? 'اختر عقارات لإضافتها' : selectedTab === 'DESTINATION' ? 'اختر وجهات' : 'اختر إعلانات'}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {isLoading ? 'جارِ التحميل...' : `${filteredItems.length} عنصر`}
          </Typography>
        </Stack>
        <Grid container spacing={2}>
          {filteredItems.map((item, idx) => (
            <Grid key={item.id || idx} item xs={12} sm={6} md={4} lg={3}>
              <Card>
                <CardMedia component="img" height={120} image={item.imageUrl || item.mainImageUrl || item.customImageUrl} alt={item.name || item.title} />
                <CardContent sx={{ py: 1.5 }}>
                  <Typography variant="subtitle2" noWrap>
                    {item.nameAr || item.name || item.title}
                  </Typography>
                  <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mt: 1 }}>
                    <Chip size="small" label={selectedTab} />
                    <Button
                      size="small"
                      variant="contained"
                      startIcon={<AddIcon />}
                      onClick={() => addItem(item)}
                      disabled={!canAddMore}
                    >
                      إضافة
                    </Button>
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Paper>
    </Box>
  );
};

export default ContentManagementPanel;