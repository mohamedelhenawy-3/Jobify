export interface Job {
  id:number;

  logo?:string;
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
