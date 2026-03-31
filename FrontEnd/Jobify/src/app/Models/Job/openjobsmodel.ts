export interface OpeningJobs {
  id: number;

  title: string;
  description: string;
  location: string;

  minSalary: number;
  maxSalary: number;

  workModeId: number;
  industryId: number;
  employmentTypeId: number;
  statusId: number;

  publiched: string; // Date comes as string from API (ISO format)
}