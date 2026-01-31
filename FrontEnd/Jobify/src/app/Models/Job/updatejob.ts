export interface Job {
  id:number;
  title: string;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  workModeId: number;
  industryId: number;
  employmentTypeId: number;
  statusId: number;
}
