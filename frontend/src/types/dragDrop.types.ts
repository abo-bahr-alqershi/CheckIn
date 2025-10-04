export interface Position {
  x: number;
  y: number;
}

export interface GridPosition {
  row: number;
  col: number;
  rowSpan: number;
  colSpan: number;
}

export interface DragItem {
  id: string;
  type: string;
  index?: number;
}

export interface DropTarget {
  id: string;
  index?: number;
  accept: string[];
  canDrop: (dragItem: DragItem) => boolean;
}