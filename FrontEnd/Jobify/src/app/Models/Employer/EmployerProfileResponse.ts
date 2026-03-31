import { CompanyDto } from "../Company/Companyresponse";

export interface EmployerProfileResponseDto {
  fullName: string;
  email: string;
  phoneNumber?: string;
  location?: string;
  linkedInUrl?: string;

  position?: string;
  bio?: string;
  profileImageUrl?: string;
  createdAt: string;

  company?: CompanyDto;
}