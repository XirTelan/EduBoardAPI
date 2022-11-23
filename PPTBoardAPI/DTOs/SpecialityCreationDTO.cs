using Microsoft.AspNetCore.Mvc;
using PPTBoardAPI.Helpers;

namespace PPTBoardAPI.DTOs
{
    public class SpecialityCreationDTO
    {
        public string Name { get; set; }
        [ModelBinder(BinderType= typeof(TypeBinder<List<int>>))]
        public List<int> DisciplineIds { get; set; }
    }
}
