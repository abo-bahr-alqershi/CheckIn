/**
 * File upload request DTO
 */
export interface FileUploadRequest {
  /** File name including extension */
  fileName: string;
  /** File content as base64 string */
  fileContent: string;
  /** MIME type */
  contentType: string;
  /** Storage path (optional) */
  storagePath?: string;
  /** Generate thumbnail (optional) */
  generateThumbnail?: boolean;
  /** Optimize image (optional) */
  optimizeImage?: boolean;
  /** Image quality (1-100) (optional) */
  quality?: number;
  /** Generate WebP version (optional) */
  generateWebP?: boolean;
}

/** Image type enum */
export type ImageType = 'Review' | 'Profile' | 'Management';

/**
 * Command to upload an image with additional data
 */
export interface UploadImageCommand {
  file: FileUploadRequest;
  /** File name without extension */
  name: string;
  /** File extension (e.g. .jpg, .png) */
  extension: string;
  /** Purpose of the image (Review, Profile, Management) */
  imageType: ImageType;
  /** Optimize image (optional) */
  optimizeImage?: boolean;
  /** Quality after optimization (1-100) (optional) */
  quality?: number;
  /** Generate thumbnail (optional) */
  generateThumbnail?: boolean;
}

export interface UploadedFileDto {
  fileName: string;
  originalFileName: string;
  contentType: string;
  size: number;
  url: string;
  relativePath: string;
  uploadedAt: string;
  uploadedBy?: string;
  fileHash?: string;
  width?: number;
  height?: number;
  duration?: string;
  description?: string;
  tags?: string;
}

export interface ProcessedImageResult {
  processedUrl: string;
  thumbnailUrl: string;
  fileSize: number;
  dimensions: string;
  mimeType: string;
} 