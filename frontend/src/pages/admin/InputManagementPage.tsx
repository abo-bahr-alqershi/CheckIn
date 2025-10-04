import React, { useState, useEffect, useCallback } from 'react';
import { Box, Typography, Tabs, Tab, Table, TableHead, TableRow, TableCell, TableBody, TextField, Checkbox, Button, Snackbar, Alert, CircularProgress, Pagination } from '@mui/material';
import { useNotificationContext } from '../../components/ui/NotificationProvider';
import { useCurrencies } from '../../hooks/useCurrencies';
import { CurrencySettingsService } from '../../services/currency-settings.service';
import type { CurrencyDto } from '../../types/currency.types';
import ImageUpload from '../../components/inputs/ImageUpload';
import { useCitySettings } from '../../hooks/useCitySettings';
import { CitySettingsService } from '../../services/city-settings.service';
import type { CityDto } from '../../types/city.types';

const InputManagementPage: React.FC = () => {
  const { showSuccess, showError } = useNotificationContext();
  const [activeTab, setActiveTab] = useState(0);
  const { currencies: fetchedItems, loading, error } = useCurrencies();
  const [items, setItems] = useState<CurrencyDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingItem, setEditingItem] = useState<CurrencyDto | null>(null);
  const [itemForm, setItemForm] = useState<{ code: string; arabicCode: string; name: string; arabicName: string; exchangeRate: number; isDefault: boolean }>({ code: '', arabicCode: '', name: '', arabicName: '', exchangeRate: 0, isDefault: false });
  // City management state
  const { cities: fetchedCities, loading: loadingCities, error: errorCities } = useCitySettings();
  const [cities, setCities] = useState<CityDto[]>([]);
  const [savingCities, setSavingCities] = useState(false);
  const [showCityModal, setShowCityModal] = useState(false);
  const [editingCity, setEditingCity] = useState<CityDto | null>(null);
  const [cityForm, setCityForm] = useState<CityDto>({ name: '', country: '', images: [] });
  // Pagination for cities and currencies
  const ITEMS_PER_PAGE = 5;
  const [cityPage, setCityPage] = useState<number>(1);
  const [currencyPage, setCurrencyPage] = useState<number>(1);
  // disable default checkbox if editing a non-default currency or adding when a default exists
  const isDefaultToggleDisabled = editingItem ? !editingItem.isDefault : items.some(i => i.isDefault);

  useEffect(() => {
    if (!loading) setItems(fetchedItems);
  }, [loading, fetchedItems]);
  // Clamp currency page when items change
  useEffect(() => {
    const maxCurrencyPage = Math.max(1, Math.ceil(items.length / ITEMS_PER_PAGE));
    if (currencyPage > maxCurrencyPage) setCurrencyPage(maxCurrencyPage);
  }, [items, currencyPage]);
  useEffect(() => {
    if (!loadingCities) setCities(fetchedCities);
  }, [loadingCities, fetchedCities]);
  // Clamp city page when cities change
  useEffect(() => {
    const maxCityPage = Math.max(1, Math.ceil(cities.length / ITEMS_PER_PAGE));
    if (cityPage > maxCityPage) setCityPage(maxCityPage);
  }, [cities, cityPage]);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  const handleValueChange = useCallback((code: string, value: string) => {
    setItems(prev => prev.map(i => i.code === code ? { ...i, exchangeRate: parseFloat(value) } : i));
  }, []);

  const handleDefaultChange = useCallback((code: string) => {
    setItems(prev => prev.map(i => ({ ...i, isDefault: i.code === code })));
  }, []);

  const openAddModal = () => {
    setEditingItem(null);
    setItemForm({ code: '', arabicCode: '', name: '', arabicName: '', exchangeRate: 0, isDefault: false });
    setShowModal(true);
  };

  const openEditModal = (item: CurrencyDto) => {
    setEditingItem(item);
    setItemForm({ code: item.code, arabicCode: item.arabicCode, name: item.name, arabicName: item.arabicName, exchangeRate: item.exchangeRate || 0, isDefault: item.isDefault });
    setShowModal(true);
  };

  const handleDeleteItem = async (code: string) => {
    if (!window.confirm('هل أنت متأكد من الحذف؟')) return;
    const newItems = items.filter(i => i.code !== code);
    setItems(newItems);
    try {
      await CurrencySettingsService.saveCurrencies(newItems);
      showSuccess('تم الحذف بنجاح');
    } catch (err: any) {
      showError(err.message || 'فشل في الحذف');
    }
  };

  const handleModalSave = async () => {
    let newItems: CurrencyDto[];
    if (editingItem) {
      newItems = items.map(i => i.code === editingItem.code ? { ...itemForm } as CurrencyDto : i);
    } else {
      newItems = [...items, itemForm as CurrencyDto];
    }
    setSaving(true);
    try {
      await CurrencySettingsService.saveCurrencies(newItems);
      setItems(newItems);
      showSuccess(editingItem ? 'تم التحديث بنجاح' : 'تم الإضافة بنجاح');
      setShowModal(false);
    } catch (err: any) {
      showError(err.message || 'فشل في الحفظ');
    } finally {
      setSaving(false);
    }
  };
 
  // City management handlers
  const openAddCityModal = () => {
    setEditingCity(null);
    setCityForm({ name: '', country: '', images: [] });
    setShowCityModal(true);
  };
  const openEditCityModal = (city: CityDto) => {
    setEditingCity(city);
    setCityForm({ ...city });
    setShowCityModal(true);
  };
  const handleDeleteCity = async (name: string) => {
    if (!window.confirm('هل أنت متأكد من الحذف؟')) return;
    const newCities = cities.filter(c => c.name !== name);
    setCities(newCities);
    try {
      await CitySettingsService.saveCities(newCities);
      showSuccess('تم الحذف بنجاح');
    } catch (err: any) {
      showError(err.message || 'فشل في الحذف');
    }
  };
  const handleCityModalSave = async () => {
    let newCities: CityDto[];
    if (editingCity) {
      newCities = cities.map(c => c.name === editingCity.name ? cityForm : c);
    } else {
      newCities = [...cities, cityForm];
    }
    setSavingCities(true);
    try {
      await CitySettingsService.saveCities(newCities);
      setCities(newCities);
      showSuccess(editingCity ? 'تم التحديث بنجاح' : 'تم الإضافة بنجاح');
      setShowCityModal(false);
    } catch (err: any) {
      showError(err.message || 'فشل في الحفظ');
    } finally {
      setSavingCities(false);
    }
  };
  // Prepare paginated data
  const pagedCities = cities.slice((cityPage - 1) * ITEMS_PER_PAGE, cityPage * ITEMS_PER_PAGE);
  const pagedCurrencies = items.slice((currencyPage - 1) * ITEMS_PER_PAGE, currencyPage * ITEMS_PER_PAGE);

  return (
    <Box className="min-h-screen bg-gray-50 p-6" dir="rtl">
      {/* Header */}
      <div className="flex justify-between items-center mb-4">
        <div>
        <h1 className="text-2xl font-bold text-gray-900">إدارة المدخلات</h1>
        <Typography className="text-gray-600">تحتوي على ادارة المدن والعملات .</Typography>
        </div>
      </div>


      {/* Statistics */}
      <Box className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <Box className="bg-blue-50 p-4 rounded-lg flex items-center">
          <Typography className="text-blue-600 text-2xl ml-3">📌</Typography>
          <Box>
            <Typography className="text-sm text-blue-600">المدن</Typography>
            <Typography className="text-2xl font-bold text-blue-900">0</Typography>
          </Box>
        </Box>
        <Box className="bg-green-50 p-4 rounded-lg flex items-center">
          <Typography className="text-green-600 text-2xl ml-3">🔧</Typography>
          <Box>
            <Typography className="text-sm text-green-600">العملات</Typography>
            <Typography className="text-2xl font-bold text-green-900">{items.length}</Typography>
          </Box>
        </Box>
        <Box className="bg-yellow-50 p-4 rounded-lg flex items-center">
          <Typography className="text-yellow-600 text-2xl ml-3">📂</Typography>
          <Box>
            <Typography className="text-sm text-yellow-600">القسم 3</Typography>
            <Typography className="text-2xl font-bold text-yellow-900">0</Typography>
          </Box>
        </Box>
        <Box className="bg-purple-50 p-4 rounded-lg flex items-center">
          <Typography className="text-purple-600 text-2xl ml-3">📝</Typography>
          <Box>
            <Typography className="text-sm text-purple-600">القسم 4</Typography>
            <Typography className="text-2xl font-bold text-purple-900">0</Typography>
          </Box>
        </Box>
      </Box>

      {/* Main Grid */}
      <Box className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Column 1: Cities */}
        <Box className="bg-white rounded-lg shadow-sm p-6">
          <Box className="flex justify-between items-center mb-4">
            <Typography variant="h6" className="font-semibold">المدن</Typography>
            <Button variant="contained" color="primary" onClick={openAddCityModal}>+ إضافة</Button>
          </Box>
          {loadingCities ? (
            <Box className="flex justify-center py-10"><CircularProgress /></Box>
          ) : errorCities ? (
            <Typography color="error" align="center">{errorCities.message}</Typography>
          ) : (
            <div className="space-y-2">
              {pagedCities.map(city => (
                <div key={city.name} className="p-3 rounded-lg border border-gray-200 hover:border-gray-300 cursor-pointer transition-colors">
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <h3 className="font-medium text-gray-900 text-sm">{city.name}</h3>
                      <p className="text-xs text-gray-600">الدولة: {city.country}</p>
                      <p className="text-xs text-gray-600">الصور: {city.images.length}</p>
                    </div>
                    <div className="flex space-x-1 space-x-reverse">
                      <button onClick={() => openEditCityModal(city)} className="text-blue-600 hover:text-blue-800 text-xs">✏️</button>
                      <button onClick={() => handleDeleteCity(city.name)} className="text-red-600 hover:text-red-800 text-xs">🗑️</button>
                    </div>
                  </div>
                </div>
              ))}
              {cities.length === 0 && (
                <div className="text-center py-10 text-gray-500">
                  <div className="text-3xl mb-2">🏙️</div>
                  <Typography className="text-sm">لا توجد مدن</Typography>
                </div>
              )}
              {cities.length > ITEMS_PER_PAGE && (
                <Box className="flex justify-center mt-2">
                  <Pagination
                    dir='ltr'
                    count={Math.ceil(cities.length / ITEMS_PER_PAGE)}
                    page={cityPage}
                    onChange={(_, v) => setCityPage(v)}
                    size="small"
                    color="primary"
                  />
                </Box>
              )}
            </div>
          )}
        </Box>

        {/* Column 2 */}
        <Box className="bg-white rounded-lg shadow-sm p-6">
          <Box className="flex justify-between items-center mb-4">
            <Typography variant="h6" className="font-semibold">العملات</Typography>
            <Button variant="contained" color="primary" onClick={openAddModal}>
              + إضافة
            </Button>
          </Box>
          {loading ? (
            <Box className="flex justify-center py-10"><CircularProgress /></Box>
          ) : error ? (
            <Typography color="error" align="center">{error.message}</Typography>
          ) : (
            <div className="space-y-2">
              {pagedCurrencies.map(item => (
                <div
                  key={item.code}
                  className="p-3 rounded-lg border border-gray-200 hover:border-gray-300 cursor-pointer transition-colors"
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <h3 className="font-medium text-gray-900 text-sm">
                        {item.name} ({item.code})
                      </h3>
                      <p className="text-xs text-gray-600">القيمة: {item.exchangeRate ?? ''}</p>
                      <p className="text-xs text-gray-600">افتراضي: {item.isDefault ? 'نعم' : 'لا'}</p>
                    </div>
                    <div className="flex space-x-1 space-x-reverse">
                      <button
                        onClick={() => openEditModal(item)}
                        className="text-blue-600 hover:text-blue-800 text-xs"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={() => handleDeleteItem(item.code)}
                        className="text-red-600 hover:text-red-800 text-xs"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                </div>
              ))}
              {items.length === 0 && (
                <div className="text-center py-10 text-gray-500">
                  <div className="text-3xl mb-2">🔧</div>
                  <Typography className="text-sm">لا توجد مدخلات</Typography>
                </div>
              )}
              {items.length > ITEMS_PER_PAGE && (
                <Box className="flex justify-center mt-2">
                  <Pagination
                    dir='ltr'
                    count={Math.ceil(items.length / ITEMS_PER_PAGE)}
                    page={currencyPage}
                    onChange={(_, v) => setCurrencyPage(v)}
                    size="small"
                    color="primary"
                  />
                </Box>
              )}
            </div>
          )}
        </Box>

        {/* Column 3 */}
        <Box className="bg-white rounded-lg shadow-sm p-6">
          <Box className="flex justify-between items-center mb-4">
            <Typography variant="h6" className="font-semibold">القسم 3</Typography>
            <Button variant="contained" disabled>+ إضافة</Button>
          </Box>
          <Box className="text-center text-gray-500 py-10">لا توجد بيانات</Box>
        </Box>

        {/* Column 4 */}
        <Box className="bg-white rounded-lg shadow-sm p-6">
          <Box className="flex justify-between items-center mb-4">
            <Typography variant="h6" className="font-semibold">القسم 4</Typography>
            <Button variant="contained" disabled>+ إضافة</Button>
          </Box>
          <Box className="text-center text-gray-500 py-10">لا توجد بيانات</Box>
        </Box>
      </Box>

      {/* Add/Edit Modal - Tailwind Styled */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">
              {editingItem ? 'تعديل المدخل' : 'إضافة مدخل جديد'}
            </h3>
            <form onSubmit={(e) => { e.preventDefault(); handleModalSave(); }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">المعرف</label>
                  <input
                    type="text"
                    value={itemForm.code}
                    onChange={e => setItemForm(prev => ({ ...prev, code: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الرمز العربي</label>
                  <input
                    type="text"
                    value={itemForm.arabicCode}
                    onChange={e => setItemForm(prev => ({ ...prev, arabicCode: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الاسم</label>
                  <input
                    type="text"
                    value={itemForm.name}
                    onChange={e => setItemForm(prev => ({ ...prev, name: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">القيمة</label>
                  <input
                    type="number"
                    value={itemForm.exchangeRate}
                    onChange={e => setItemForm(prev => ({ ...prev, exchangeRate: parseFloat(e.target.value) || 0 }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <div className="flex items-center space-x-2 space-x-reverse">
                  <Checkbox
                    checked={itemForm.isDefault}
                    disabled={isDefaultToggleDisabled}
                    onChange={e => setItemForm(prev => ({ ...prev, isDefault: e.target.checked }))}
                  />
                  <label className="text-sm font-medium text-gray-700">افتراضي</label>
                </div>
              </div>
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
                >
                  {editingItem ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
      {showCityModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-lg font-semibold mb-4">
              {editingCity ? 'تعديل المدينة' : 'إضافة مدينة جديدة'}
            </h3>
            <form onSubmit={e => { e.preventDefault(); handleCityModalSave(); }}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">اسم المدينة</label>
                  <input
                    type="text"
                    value={cityForm.name}
                    onChange={e => setCityForm(prev => ({ ...prev, name: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الدولة</label>
                  <input
                    type="text"
                    value={cityForm.country}
                    onChange={e => setCityForm(prev => ({ ...prev, country: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">صور المدينة</label>
                  <ImageUpload
                    value={cityForm.images}
                    multiple={true}
                    onChange={urls => setCityForm(prev => ({
                      ...prev,
                      images: Array.isArray(urls) ? urls : [urls]
                    }))}
                  />
                </div>
              </div>
              <div className="flex justify-end space-x-2 space-x-reverse mt-6">
                <button
                  type="button"
                  onClick={() => setShowCityModal(false)}
                  className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                >
                  إلغاء
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
                >
                  {editingCity ? 'تحديث' : 'إضافة'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Box>
  );
};

export default InputManagementPage; 