// src/components/availability-pricing/pricing/PricingQuickActions.tsx

import React from 'react';
import { Button } from '../ui/Button';
import { TrendingUp, TrendingDown, Edit, Layers } from 'lucide-react';

interface PricingQuickActionsProps {
  selectedCount: number;
  onIncrease: () => void;
  onDecrease: () => void;
  onUpdate: () => void;
  onBulkOperations: () => void;
}

export const PricingQuickActions: React.FC<PricingQuickActionsProps> = ({
  selectedCount,
  onIncrease,
  onDecrease,
  onUpdate,
  onBulkOperations
}) => {
  return (
    <div className="flex items-center gap-2">
      <Button
        size="sm"
        variant="outline"
        onClick={onIncrease}
        icon={<TrendingUp className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        +10%
      </Button>
      <Button
        size="sm"
        variant="outline"
        onClick={onDecrease}
        icon={<TrendingDown className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        -10%
      </Button>
      <Button
        size="sm"
        variant="outline"
        onClick={onUpdate}
        icon={<Edit className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        تحديث
      </Button>
      <Button
        size="sm"
        variant="primary"
        onClick={onBulkOperations}
        icon={<Layers className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        عمليات متقدمة
      </Button>
    </div>
  );
};