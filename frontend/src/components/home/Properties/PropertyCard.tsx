// frontend/src/components/Properties/PropertyCard.tsx
import React from 'react';
import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  Box,
  Chip,
  Rating,
  IconButton,
} from '@mui/material';
import { Favorite, FavoriteBorder, LocationOn } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface PropertyCardProps {
  property: any;
  variant?: 'horizontal' | 'vertical';
  showBadge?: boolean;
  showPrice?: boolean;
  showRating?: boolean;
}

const PropertyCard: React.FC<PropertyCardProps> = ({
  property,
  variant = 'vertical',
  showBadge = true,
  showPrice = true,
  showRating = true,
}) => {
  const navigate = useNavigate();
  const [isFavorite, setIsFavorite] = React.useState(false);

  const handleClick = () => {
    navigate(`/properties/${property.id}`);
  };

  const handleFavoriteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsFavorite(!isFavorite);
    // TODO: Call API to save/remove favorite
  };

  const displayRating = typeof property.averageRating === 'number' ? property.averageRating : undefined;
  const displayPrice = typeof property.minPrice === 'number' ? property.minPrice : (typeof property.basePrice === 'number' ? property.basePrice : undefined);
  const currency = property.currency || 'ريال';

  return (
    <motion.div whileHover={{ y: -5 }} transition={{ duration: 0.2 }}>
      <Card
        sx={{
          cursor: 'pointer',
          height: '100%',
          display: 'flex',
          flexDirection: 'column',
          '&:hover': {
            boxShadow: 6,
          },
        }}
        onClick={handleClick}
      >
        <Box sx={{ position: 'relative' }}>
          <CardMedia
            component="img"
            height={variant === 'horizontal' ? 200 : 240}
            image={property.mainImageUrl || property.images?.[0]}
            alt={property.name}
          />
          
          {/* زر المفضلة */}
          <IconButton
            sx={{
              position: 'absolute',
              top: 8,
              right: 8,
              backgroundColor: 'rgba(255, 255, 255, 0.9)',
              '&:hover': {
                backgroundColor: 'rgba(255, 255, 255, 1)',
              },
            }}
            onClick={handleFavoriteClick}
          >
            {isFavorite ? <Favorite color="error" /> : <FavoriteBorder />}
          </IconButton>
          
          {/* الشارات */}
          {showBadge && (
            <Box sx={{ position: 'absolute', top: 8, left: 8, display: 'flex', gap: 0.5 }}>
              {property.badgeText && (
                <Chip
                  label={property.badgeText}
                  size="small"
                  sx={{
                    backgroundColor: property.badgeColor || 'primary.main',
                    color: 'white',
                    fontWeight: 'bold',
                  }}
                />
              )}
              {property.discountPercentage && (
                <Chip
                  label={`${property.discountPercentage}% خصم`}
                  size="small"
                  sx={{
                    backgroundColor: 'error.main',
                    color: 'white',
                    fontWeight: 'bold',
                  }}
                />
              )}
            </Box>
          )}
        </Box>
        
        <CardContent sx={{ flexGrow: 1, pb: 2 }}>
          <Typography variant="h6" component="h3" gutterBottom noWrap>
            {property.name}
          </Typography>
          
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
            <LocationOn sx={{ fontSize: 16, color: 'text.secondary', mr: 0.5 }} />
            <Typography variant="body2" color="text.secondary" noWrap>
              {property.city}
            </Typography>
          </Box>
          
          {showRating && typeof displayRating === 'number' && (
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
              <Rating
                value={displayRating}
                precision={0.5}
                size="small"
                readOnly
              />
              <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
                ({displayRating})
              </Typography>
            </Box>
          )}
          
          {showPrice && typeof displayPrice === 'number' && (
            <Box sx={{ mt: 'auto' }}>
              <Typography variant="h6" color="primary" fontWeight="bold">
                {displayPrice.toLocaleString()} {currency}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                لليلة الواحدة
              </Typography>
            </Box>
          )}
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default PropertyCard;