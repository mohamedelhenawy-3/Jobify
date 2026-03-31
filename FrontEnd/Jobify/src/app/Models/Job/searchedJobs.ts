export interface Job {
  id:number;

  logo?:string;
  title: string;
  description: string;
  location: string;
  workModeId: number;
  industryId: number;
  employmentTypeId: number;
  statusId: number;
  data:string

}
