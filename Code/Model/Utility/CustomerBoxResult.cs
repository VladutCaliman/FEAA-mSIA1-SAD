using Code.ViewModel.Utility;

namespace Code.Model.Utility
{
    internal sealed record CustomerBoxResult(
         CustomerCriteriaEnum CustomerCriteria,
         int QueryRange);
}
