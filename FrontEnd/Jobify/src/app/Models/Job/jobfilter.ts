export interface JobFilter {
  keyword?: string;
  industryIds?: number[];
  employmentTypeIds?: number[];
  workModeIds?: number[];
  postedWithinDays?: number;
}
