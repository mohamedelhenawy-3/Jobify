import { SearchCompany } from "../Company/Search";
import { SearchJob } from "../Job/search";
import { SearchUser } from "../jobseeker/searchuser";

export interface GlobalSearchResponse {
  jobs: SearchJob[];
  users: SearchUser[];
  companies: SearchCompany[];
}