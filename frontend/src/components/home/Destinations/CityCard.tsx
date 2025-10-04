// frontend/src/components/Destinations/CityCard.tsx
import React from 'react';
import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  Box,
  Chip,
} from '@mui/material';
import { Place, Hotel } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface CityCardProps {
  city: any;
}

const CityCard: React.FC<CityCardProps> = ({ city }) => {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate(`/destinations/${city.id}`);
  };

  return (
    <motion.div whileHover={{ scale: 1.02 }} transition={{ duration: 0.2 }}>
      <Card
        sx={{
          cursor: 'pointer',
          position: 'relative',
          overflow: 'hidden',
          '&:hover': {
            '& .city-overlay': {
              backgroundColor: 'rgba(0, 0, 0, 0.5)',
            },
          },
        }}
        onClick={handleClick}
      >
        <CardMedia
          component="img"
          height={280}
          image={city.imageUrl}
          alt={city.nameAr || city.name}
        />
        
        <Box
          className="city-overlay"
          sx={{
            position: 'absolute',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.3)',
            transition: 'background-color 0.3s ease',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'flex-end',
            p: 3,
          }}
        >
          <Box>
            {city.isPopular && (
              <Chip
                label="وجهة شائعة"
                size="small"
                sx={{
                  backgroundColor: 'warning.main',
                  color: 'white',
                  mb: 2,
                }}
              />
            )}
            
            <Typography variant="h4" color="white" fontWeight="bold" gutterBottom>
              {city.nameAr || city.name}
            </Typography>
            
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Place sx={{ color: 'white', mr: 0.5 }} />
                <Typography variant="body1" color="white">
                  {city.countryAr || city.country}
                </Typography>
              </Box>
              
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Hotel sx={{ color: 'white', mr: 0.5 }} />
                <Typography variant="body1" color="white">
                  {city.propertyCount} عقار
                </Typography>
              </Box>
            </Box>
            
            {city.averagePrice && (
              <Typography variant="body2" color="white" sx={{ mt: 1 }}>
                متوسط السعر: {city.averagePrice.toLocaleString()} {city.currency}
              </Typography>
            )}
          </Box>
        </Box>
      </Card>
    </motion.div>
  );
};

export default CityCard;