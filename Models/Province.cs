using System;
using System.Collections.Generic;

namespace DevHub.Models;

// A Vietnamese province/city. Linked to job posts via the job_post_province
// many-to-many junction (a job post can target several provinces).
public partial class Province
{
    public int ProvinceId { get; set; }

    public string ProvinceName { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}
