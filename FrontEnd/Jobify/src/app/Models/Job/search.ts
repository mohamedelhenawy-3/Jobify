export interface SearchJob {
  id: number;
  logo: string;
  title: string;
  description: string | null;
  location: string;
  workModeId: number;
  industryId: number;
  employmentTypeId: number;
  statusId: number;
  date: string;
}