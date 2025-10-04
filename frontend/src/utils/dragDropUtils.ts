import type { DragItem, DropTarget, Position, GridPosition } from '../types/dragDrop.types';
import type { HomeScreenComponent, HomeScreenSection } from '../types/homeScreen.types';

export const GRID_SIZE = 12;
export const CELL_HEIGHT = 80;
export const GAP_SIZE = 16;

/**
 * Calculate grid position from mouse coordinates
 */
export const getGridPosition = (
  mouseX: number,
  mouseY: number,
  containerRect: DOMRect,
  gridColumns: number = GRID_SIZE
): GridPosition => {
  const relativeX = mouseX - containerRect.left;
  const relativeY = mouseY - containerRect.top;
  
  const cellWidth = (containerRect.width - (gridColumns - 1) * GAP_SIZE) / gridColumns;
  
  const col = Math.floor(relativeX / (cellWidth + GAP_SIZE));
  const row = Math.floor(relativeY / (CELL_HEIGHT + GAP_SIZE));
  
  return {
    row: Math.max(0, row),
    col: Math.max(0, Math.min(col, gridColumns - 1)),
    rowSpan: 1,
    colSpan: 1
  };
};

/**
 * Check if component position is valid within the grid
 */
export const isValidPosition = (
  position: GridPosition,
  existingComponents: HomeScreenComponent[],
  componentId?: string
): boolean => {
  // Check grid boundaries
  if (position.col + position.colSpan > GRID_SIZE) return false;
  if (position.row < 0 || position.col < 0) return false;
  
  // Check collisions with existing components
  return !existingComponents.some(component => {
    if (component.id === componentId) return false;
    
    const componentEndCol = component.colSpan;
    const positionEndCol = position.col + position.colSpan;
    
    // Check horizontal overlap
    const horizontalOverlap = !(position.col >= componentEndCol || positionEndCol <= component.order);
    
    return horizontalOverlap;
  });
};

/**
 * Find nearest valid drop position
 */
export const findNearestValidPosition = (
  targetPosition: GridPosition,
  existingComponents: HomeScreenComponent[],
  componentSize: { colSpan: number; rowSpan: number }
): GridPosition | null => {
  // Try target position first
  const candidatePosition = { ...targetPosition, ...componentSize };
  if (isValidPosition(candidatePosition, existingComponents)) {
    return candidatePosition;
  }
  
  // Search in expanding radius
  for (let radius = 1; radius <= GRID_SIZE; radius++) {
    for (let dx = -radius; dx <= radius; dx++) {
      for (let dy = -radius; dy <= radius; dy++) {
        if (Math.abs(dx) !== radius && Math.abs(dy) !== radius) continue;
        
        const newPosition: GridPosition = {
          row: targetPosition.row + dy,
          col: targetPosition.col + dx,
          ...componentSize
        };
        
        if (isValidPosition(newPosition, existingComponents)) {
          return newPosition;
        }
      }
    }
  }
  
  return null;
};

/**
 * Get drop zone indicator position
 */
export const getDropIndicatorStyle = (
  dropPosition: GridPosition,
  containerWidth: number
): React.CSSProperties => {
  const cellWidth = (containerWidth - (GRID_SIZE - 1) * GAP_SIZE) / GRID_SIZE;
  
  return {
    position: 'absolute',
    left: dropPosition.col * (cellWidth + GAP_SIZE),
    top: dropPosition.row * (CELL_HEIGHT + GAP_SIZE),
    width: dropPosition.colSpan * cellWidth + (dropPosition.colSpan - 1) * GAP_SIZE,
    height: dropPosition.rowSpan * CELL_HEIGHT + (dropPosition.rowSpan - 1) * GAP_SIZE,
    backgroundColor: 'rgba(59, 130, 246, 0.2)',
    border: '2px dashed #3b82f6',
    borderRadius: '8px',
    pointerEvents: 'none',
    transition: 'all 0.2s ease'
  };
};

/**
 * Sort components by position for proper rendering order
 */
export const sortComponentsByPosition = (components: HomeScreenComponent[]): HomeScreenComponent[] => {
  return [...components].sort((a, b) => {
    // Sort by row first, then by column
    if (a.order !== b.order) return a.order - b.order;
    return 0;
  });
};

/**
 * Calculate component insertion index
 */
export const getInsertionIndex = (
  components: HomeScreenComponent[],
  dropPosition: Position,
  containerRect: DOMRect
): number => {
  const sorted = sortComponentsByPosition(components);
  const gridPos = getGridPosition(dropPosition.x, dropPosition.y, containerRect);
  
  return sorted.findIndex(component => {
    return component.order > gridPos.row;
  });
};

/**
 * Create drag preview element
 */
export const createDragPreview = (element: HTMLElement): HTMLDivElement => {
  const preview = document.createElement('div');
  preview.className = 'drag-preview';
  preview.style.cssText = `
    position: fixed;
    pointer-events: none;
    z-index: 9999;
    opacity: 0.8;
    transform: rotate(2deg);
    transition: transform 0.2s ease;
  `;
  
  const clone = element.cloneNode(true) as HTMLElement;
  clone.style.width = `${element.offsetWidth}px`;
  clone.style.height = `${element.offsetHeight}px`;
  
  preview.appendChild(clone);
  document.body.appendChild(preview);
  
  return preview;
};

/**
 * Update drag preview position
 */
export const updateDragPreview = (preview: HTMLElement, x: number, y: number): void => {
  preview.style.left = `${x - preview.offsetWidth / 2}px`;
  preview.style.top = `${y - preview.offsetHeight / 2}px`;
};

/**
 * Clean up drag preview
 */
export const removeDragPreview = (preview: HTMLElement): void => {
  preview.remove();
};

/**
 * Check if drag item can be dropped on target
 */
export const canDrop = (dragItem: DragItem, dropTarget: DropTarget): boolean => {
  // Check if target accepts the drag item type
  if (!dropTarget.accept.includes(dragItem.type)) return false;
  
  // Additional validation
  return dropTarget.canDrop(dragItem);
};

/**
 * Reorder array items
 */
export const reorderArray = <T>(
  array: T[],
  sourceIndex: number,
  destinationIndex: number
): T[] => {
  const result = Array.from(array);
  const [removed] = result.splice(sourceIndex, 1);
  result.splice(destinationIndex, 0, removed);
  return result;
};

/**
 * Move item between arrays
 */
export const moveItemBetweenArrays = <T>(
  sourceArray: T[],
  destinationArray: T[],
  sourceIndex: number,
  destinationIndex: number
): { source: T[]; destination: T[] } => {
  const sourceClone = Array.from(sourceArray);
  const destClone = Array.from(destinationArray);
  
  const [removed] = sourceClone.splice(sourceIndex, 1);
  destClone.splice(destinationIndex, 0, removed);
  
  return {
    source: sourceClone,
    destination: destClone
  };
};