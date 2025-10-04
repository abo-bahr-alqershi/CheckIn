// frontend/src/components/common/ColorPicker.tsx
import React, { useState } from 'react';
import { HexColorPicker } from 'react-colorful';
import {
  Box,
  Popover,
  TextField,
  Paper,
  Stack,
  Typography,
  Button,
  InputAdornment,
  IconButton,
  Grid,
} from '@mui/material';
import {
  ColorLens as ColorLensIcon,
  Check as CheckIcon,
  Palette as PaletteIcon,
} from '@mui/icons-material';

interface ColorPickerProps {
  value: string;
  onChange: (color: string) => void;
  label?: string;
  disabled?: boolean;
  fullWidth?: boolean;
}

const PRESET_COLORS = {
  'أساسية': [
    '#ffffff', '#f5f5f5', '#e0e0e0', '#9e9e9e', 
    '#757575', '#424242', '#212121', '#000000',
  ],
  'ألوان رئيسية': [
    '#f44336', '#e91e63', '#9c27b0', '#673ab7',
    '#3f51b5', '#2196f3', '#03a9f4', '#00bcd4',
  ],
  'ألوان ثانوية': [
    '#009688', '#4caf50', '#8bc34a', '#cddc39',
    '#ffeb3b', '#ffc107', '#ff9800', '#ff5722',
  ],
  'ألوان داكنة': [
    '#b71c1c', '#880e4f', '#4a148c', '#311b92',
    '#1a237e', '#0d47a1', '#01579b', '#004d40',
  ],
};

const ColorPicker: React.FC<ColorPickerProps> = ({
  value = '#000000',
  onChange,
  label = 'اختر اللون',
  disabled = false,
  fullWidth = false,
}) => {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [tempColor, setTempColor] = useState(value);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    if (!disabled) {
      setAnchorEl(event.currentTarget);
      setTempColor(value);
    }
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleColorSelect = (color: string) => {
    setTempColor(color);
    onChange(color);
  };

  const handleApply = () => {
    onChange(tempColor);
    handleClose();
  };

  const open = Boolean(anchorEl);

  return (
    <>
      <TextField
        label={label}
        value={value}
        onClick={handleClick}
        disabled={disabled}
        fullWidth={fullWidth}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <Box
                sx={{
                  width: 28,
                  height: 28,
                  backgroundColor: value,
                  border: '2px solid',
                  borderColor: 'divider',
                  borderRadius: 1,
                  cursor: disabled ? 'default' : 'pointer',
                  boxShadow: 'inset 0 0 0 1px rgba(0,0,0,0.1)',
                }}
              />
            </InputAdornment>
          ),
          endAdornment: (
            <InputAdornment position="end">
              <IconButton
                size="small"
                onClick={handleClick}
                disabled={disabled}
              >
                <PaletteIcon />
              </IconButton>
            </InputAdornment>
          ),
          readOnly: true,
        }}
      />

      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
      >
        <Paper sx={{ p: 3, width: 360, maxHeight: '80vh', overflow: 'auto' }}>
          <Stack spacing={3}>
            <Stack direction="row" alignItems="center" justifyContent="space-between">
              <Typography variant="h6" sx={{ fontWeight: 600 }}>
                اختر اللون
              </Typography>
              <IconButton size="small" onClick={handleClose}>
                ×
              </IconButton>
            </Stack>

            {/* React Colorful Picker */}
            <Box sx={{ 
              '& .react-colorful': { 
                width: '100%',
                height: 200,
              }
            }}>
              <HexColorPicker color={tempColor} onChange={setTempColor} />
            </Box>

            {/* Color Input */}
            <TextField
              size="small"
              value={tempColor}
              onChange={(e) => {
                const value = e.target.value;
                if (/^#[0-9A-Fa-f]{0,6}$/.test(value)) {
                  setTempColor(value);
                }
              }}
              placeholder="#000000"
              fullWidth
              label="قيمة اللون (HEX)"
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Box
                      sx={{
                        width: 20,
                        height: 20,
                        backgroundColor: tempColor,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 0.5,
                      }}
                    />
                  </InputAdornment>
                ),
              }}
            />

            {/* Preset Colors */}
            {Object.entries(PRESET_COLORS).map(([category, colors]) => (
              <Box key={category}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 500 }}>
                  {category}
                </Typography>
                <Grid container spacing={0.5}>
                  {colors.map((color) => (
                    <Grid item key={color}>
                      <Box
                        onClick={() => handleColorSelect(color)}
                        sx={{
                          width: 36,
                          height: 36,
                          backgroundColor: color,
                          border: tempColor === color ? '3px solid #1976d2' : '1px solid',
                          borderColor: tempColor === color ? 'primary.main' : 'divider',
                          borderRadius: 0.5,
                          cursor: 'pointer',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          transition: 'all 0.2s',
                          '&:hover': {
                            transform: 'scale(1.1)',
                            boxShadow: 2,
                          },
                        }}
                      >
                        {tempColor === color && (
                          <CheckIcon
                            sx={{
                              fontSize: 18,
                              color: ['#ffffff', '#f5f5f5', '#e0e0e0', '#ffeb3b', '#ffc107', '#cddc39'].includes(color) 
                                ? '#000' 
                                : '#fff',
                            }}
                          />
                        )}
                      </Box>
                    </Grid>
                  ))}
                </Grid>
              </Box>
            ))}

            {/* Action Buttons */}
            <Stack direction="row" spacing={2} justifyContent="flex-end">
              <Button onClick={handleClose} variant="outlined">
                إلغاء
              </Button>
              <Button
                variant="contained"
                onClick={handleApply}
                startIcon={<CheckIcon />}
              >
                تطبيق
              </Button>
            </Stack>
          </Stack>
        </Paper>
      </Popover>
    </>
  );
};

export default ColorPicker;