// frontend/src/components/Common/CountdownTimer.tsx
import React, { useState, useEffect } from 'react';
import { Box, Typography, Paper } from '@mui/material';

interface CountdownTimerProps {
  endDate: string;
}

interface TimeLeft {
  days: number;
  hours: number;
  minutes: number;
  seconds: number;
}

const CountdownTimer: React.FC<CountdownTimerProps> = ({ endDate }) => {
  const calculateTimeLeft = (): TimeLeft => {
    const difference = +new Date(endDate) - +new Date();
    
    if (difference > 0) {
      return {
        days: Math.floor(difference / (1000 * 60 * 60 * 24)),
        hours: Math.floor((difference / (1000 * 60 * 60)) % 24),
        minutes: Math.floor((difference / 1000 / 60) % 60),
        seconds: Math.floor((difference / 1000) % 60),
      };
    }
    
    return { days: 0, hours: 0, minutes: 0, seconds: 0 };
  };

  const [timeLeft, setTimeLeft] = useState<TimeLeft>(calculateTimeLeft());

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeLeft(calculateTimeLeft());
    }, 1000);

    return () => clearInterval(timer);
  }, [endDate]);

  const TimeUnit: React.FC<{ value: number; label: string }> = ({ value, label }) => (
    <Paper
      elevation={2}
      sx={{
        p: 2,
        textAlign: 'center',
        minWidth: 60,
        backgroundColor: 'error.main',
        color: 'white',
      }}
    >
      <Typography variant="h5" fontWeight="bold">
        {value.toString().padStart(2, '0')}
      </Typography>
      <Typography variant="caption">{label}</Typography>
    </Paper>
  );

  return (
    <Box sx={{ display: 'flex', gap: 1 }}>
      <TimeUnit value={timeLeft.days} label="يوم" />
      <TimeUnit value={timeLeft.hours} label="ساعة" />
      <TimeUnit value={timeLeft.minutes} label="دقيقة" />
      <TimeUnit value={timeLeft.seconds} label="ثانية" />
    </Box>
  );
};

export default CountdownTimer;