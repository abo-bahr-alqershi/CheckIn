// frontend/src/components/Offers/OfferCard.tsx
import React, { useEffect, useState } from 'react';
import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  Box,
  Button,
  Chip,
  LinearProgress,
} from '@mui/material';
import { Timer, LocalOffer } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import CountdownTimer from '../Common/CountdownTimer';

interface OfferCardProps {
  offer: any;
  fullWidth?: boolean;
}

const OfferCard: React.FC<OfferCardProps> = ({ offer, fullWidth = false }) => {
  const navigate = useNavigate();
  
  const handleCTAClick = () => {
    if (offer.ctaAction === 'navigate' && offer.ctaData?.route) {
      navigate(offer.ctaData.route);
    } else if (offer.ctaAction === 'open_url' && offer.ctaData?.url) {
      window.open(offer.ctaData.url, '_blank');
    }
  };

  const isLimitedTime = offer.endDate && new Date(offer.endDate) > new Date();
  
  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.3 }}
    >
      <Card
        sx={{
          position: 'relative',
          overflow: 'hidden',
          backgroundColor: offer.backgroundColor || 'background.paper',
          width: fullWidth ? '100%' : 'auto',
        }}
      >
        {offer.customImageUrl && (
          <CardMedia
            component="img"
            height={fullWidth ? 300 : 200}
            image={offer.customImageUrl}
            alt={offer.title}
          />
        )}
        
        <CardContent>
          <Box sx={{ mb: 2 }}>
            <Typography
              variant="h5"
              fontWeight="bold"
              color={offer.textColor || 'text.primary'}
              gutterBottom
            >
              {offer.title}
            </Typography>
            
            {offer.subtitle && (
              <Typography
                variant="body1"
                color={offer.textColor || 'text.secondary'}
                gutterBottom
              >
                {offer.subtitle}
              </Typography>
            )}
          </Box>
          
          {offer.description && (
            <Typography
              variant="body2"
              color={offer.textColor || 'text.secondary'}
              paragraph
              sx={{ mb: 3 }}
            >
              {offer.description}
            </Typography>
          )}
          
          {/* عداد الوقت للعروض المحدودة */}
          {isLimitedTime && (
            <Box sx={{ mb: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                <Timer sx={{ mr: 1, color: 'error.main' }} />
                <Typography variant="body2" color="error.main" fontWeight="medium">
                  العرض ينتهي خلال:
                </Typography>
              </Box>
              <CountdownTimer endDate={offer.endDate} />
            </Box>
          )}
          
          {/* معلومات العقار المرتبط */}
          {offer.property && (
            <Box
              sx={{
                p: 2,
                borderRadius: 2,
                backgroundColor: 'grey.100',
                mb: 3,
              }}
            >
              <Typography variant="subtitle2" fontWeight="medium" gutterBottom>
                {offer.property.name}
              </Typography>
              {offer.property.basePrice && (
                <Box sx={{ display: 'flex', alignItems: 'baseline', gap: 1 }}>
                  <Typography variant="h6" color="primary" fontWeight="bold">
                    {offer.property.basePrice.toLocaleString()} {offer.property.currency}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    لليلة الواحدة
                  </Typography>
                </Box>
              )}
            </Box>
          )}
          
          <Button
            variant="contained"
            size="large"
            fullWidth
            onClick={handleCTAClick}
            sx={{
              backgroundColor: offer.styling?.ctaBackgroundColor || 'primary.main',
              color: offer.styling?.ctaTextColor || 'white',
              '&:hover': {
                backgroundColor: offer.styling?.ctaHoverColor || 'primary.dark',
              },
            }}
            startIcon={<LocalOffer />}
          >
            {offer.ctaText}
          </Button>
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default OfferCard;