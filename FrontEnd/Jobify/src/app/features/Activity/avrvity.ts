 
export interface ActivityDto {
  id: number;
  type: string;
  category: string;
  message: string;
  actorEmployerId: number | null;
  actorName: string;
  targetName: string | null;
  refId: number | null;
  createdAt: string;
}
 
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
 
export interface ActivityFilter {
  search: string;
  category: string;
  from: string;   // yyyy-MM-dd
  to: string;     // yyyy-MM-dd
  actorEmployerId?: number | null;   // company feed only
  pageNumber: number;
  pageSize: number;
}