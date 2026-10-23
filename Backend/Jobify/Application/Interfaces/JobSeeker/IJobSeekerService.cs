using Application.DTOs.JobSeeker;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.DTOs.JobSeeker.JobSeekers.Updates;
using Application.DTOs.SuccefullOp;
using Application.SearchFeature.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.JobSeeker
{
    public interface IJobSeekerService
    {
        Task<ProfileDto> GetMyAsync();
        Task<ProfileDto> UpdateBasicAsync(UpdateBasicDto dto);
        Task<ProfileDto> UpdateAboutAsync(UpdateAboutDto dto);
        Task<ProfileDto> SaveExperienceAsync(int? id, ExperienceDto dto);
        Task<ProfileDto> DeleteExperienceAsync(int id);
        Task<ProfileDto> SaveEducationAsync(int? id, EducationDto dto);
        Task<ProfileDto> DeleteEducationAsync(int id);
        Task<ProfileDto> SaveSkillsAsync(UpdateSkillsDto dto);

        Task<ProfileDto> SaveLanguageAsync(int? id, LanguageDto dto);
        Task<ProfileDto> DeleteLanguageAsync(int id);
        Task<ProfileDto> SaveCertificateAsync(int? id, CertificateDto dto);
        Task<ProfileDto> DeleteCertificateAsync(int id);
        Task<ProfileDto> AddProjectImagesAsync(int projectId, List<IFormFile> files);
        Task<ProfileDto> DeleteProjectImageAsync(int projectId, int imageId);
        Task<ProfileDto> SetProjectCoverAsync(int projectId, int imageId);
        Task<ProfileDto> SaveProjectAsync(int? id, ProjectDto dto);
        Task<ProfileDto> DeleteProjectAsync(int id);

        Task<ProfileDto> SavePreferencesAsync(UpdatePreferencesDto dto);
        Task<ProfileDto> UpdateProfileImge(UpdateProfileImage dto);
        Task<SuccessResponse> UploadCv(UploadMyResume resumedto);
        Task<SuccessResponse> RemoveCv();
        Task<ResumeDto> ViewMyCv();
        Task<JobSeekerProfile> GetJobSeekerProfile(int id);
        Task<SuccessResponse> SaveVisibilityAsync(UpdateVisibilityDto dto);
        Task<PublicProfileDto> GetPublicAsync(int id);
            
        Task<List<UserSeachDto>> SearchUsersAsync(GlobalSearchDto dto);

    }
}
