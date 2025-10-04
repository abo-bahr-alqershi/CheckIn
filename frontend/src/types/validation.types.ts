export interface ValidationWarningDto {
  message: string;
  warningType: FieldValidationWarningType;
  fieldId?: string;
  fieldName?: string;
  recommendations: ValidationRecommendationDto[];
}

export type FieldValidationWarningType = {
  MissingRequiredField: 0,
  InconsistentData: 1,
  LowQualityContent: 2,
  IncompleteConfiguration: 3,
  PerformanceIssue: 4,
  SecurityVulnerability: 5,
  Other: 6,
}

export interface ValidationRecommendationDto {
  recommendationType: RecommendationType;
  priority: RecommendationPriority;
  description: string;
  actionableSteps: string[];
}

export type RecommendationType = {
  DataCorrection: 0,
  ContentImprovement: 1,
  ConfigurationAdjustment: 2,
  PerformanceOptimization: 3,
  SecurityFix: 4,
  Other: 5,
}

export type RecommendationPriority = {
  High: 0,
  Medium: 1,
  Low: 2,
}

export interface ValidationStatisticsDto {
  totalWarnings: number;
  totalErrors: number;
  warningsByType: Record<string, number>;
  errorsByType: Record<string, number>;
  warningsByField: Record<string, number>;
  errorsByField: Record<string, number>;
}

export interface ValidationSettingsDto {
  enableRealtimeValidation: boolean;
  validationThreshold: number;
  excludedFields: string[];
}

export interface ValidationScope {
  scopeType: string;
  scopeId?: string;
}

export interface ValidationResult {
  isValid: boolean;
  errors: ValidationErrorDto[];
  warnings: ValidationWarningDto[];
  validationStatistics: ValidationStatisticsDto;
}

export interface ValidationErrorDto {
  message: string;
  errorType: FieldValidationErrorType;
  fieldId?: string;
  fieldName?: string;
  severity: ErrorSeverity;
}

export type FieldValidationErrorType = {
  InvalidFormat: 0,
  OutOfRange: 1,
  MissingDependency: 2,
  DuplicateEntry: 3,
  BusinessRuleViolation: 4,
  SystemError: 5,
  Other: 6,
}

export type ErrorSeverity = {
  Critical: 0,
  High: 1,
  Medium: 2,
  Low: 3,
}

export interface QualityTrend {
  date: string;
  qualityScore: number;
}

export interface ProblematicFieldDto {
  fieldId: string;
  fieldName: string;
  errorCount: number;
  warningCount: number;
  lastErrorDate?: string;
}

export interface FieldValueStatisticsDto {
  fieldId: string;
  fieldName: string;
  valueCounts: Record<string, number>;
  uniqueValues: number;
  mostCommonValue?: string;
  leastCommonValue?: string;
}

export interface FieldValidationSummaryDto {
  fieldId: string;
  fieldName: string;
  totalErrors: number;
  totalWarnings: number;
  lastValidationDate: string;
  overallQualityScore: number;
}

export interface FieldValidationReportDto {
  reportId: string;
  reportDate: string;
  validatedEntityId: string;
  validatedEntityType: string;
  validationResult: ValidationResult;
  summary: FieldValidationSummaryDto[];
  qualityTrends: QualityTrend[];
  problematicFields: ProblematicFieldDto[];
}