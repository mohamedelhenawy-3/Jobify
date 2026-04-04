export interface GlobalSearchDto {
  keyword?: string;
  type: 'all' | 'job' | 'user' | 'company';

  industryIds?: number[];
  workModeIds?: number[];
  employmentTypeIds?: number[];
  postedWithinDays?: number;

  industry?: string;
  companySize?: string;

  location?: string;
}