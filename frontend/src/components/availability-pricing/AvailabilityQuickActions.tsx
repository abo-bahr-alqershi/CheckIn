// src/components/availability-pricing/availability/AvailabilityQuickActions.tsx

import React from 'react';
import { Button } from '../ui/Button';
import { ShieldCheck, CheckCircle, Edit, Layers } from 'lucide-react';

interface AvailabilityQuickActionsProps {
  selectedCount: number;
  onBlock: () => void;
  onAvailable: () => void;
  onUpdate: () => void;
  onBulkOperations: () => void;
}

export const AvailabilityQuickActions: React.FC<AvailabilityQuickActionsProps> = ({
  selectedCount,
  onBlock,
  onAvailable,
  onUpdate,
  onBulkOperations
}) => {
  return (
    <div className="flex items-center gap-2">
      <Button
        variant="outline"
        size="sm"
        onClick={onAvailable}
        icon={<CheckCircle className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        تعيين كمتاحة
      </Button>
      <Button
        variant="outline"
        size="sm"
        onClick={onBlock}
        icon={<ShieldCheck className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        حظر سريع
      </Button>
      <Button
        variant="outline"
        size="sm"
        onClick={onUpdate}
        icon={<Edit className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        تحديث
      </Button>
      <Button
        variant="primary"
        size="sm"
        onClick={onBulkOperations}
        icon={<Layers className="h-4 w-4" />}
        disabled={selectedCount === 0}
      >
        عمليات متعددة
      </Button>
    </div>
  );
};