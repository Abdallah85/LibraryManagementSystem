using System.Linq.Expressions;

namespace Services.Specifications
{
    public class GeneralSpecifications<T> : BaseSpecifications<T> where T : class
    {
        public GeneralSpecifications() : base(null) { }
        public GeneralSpecifications(int PageIndex, int PageSize) : base(null)
        {
            ApplyPagination(PageSize, PageIndex);
        }
        public GeneralSpecifications(Expression<Func<T, bool>> expression) : base(expression) { }
        public GeneralSpecifications(Expression<Func<T, bool>> expression, int PageIndex, int PageSize) : base(expression)
        {
            ApplyPagination(PageSize, PageIndex);
        }

        // Publicly expose AddInclude as in BaseSpecifications
        public new GeneralSpecifications<T> AddInclude(Expression<Func<T, object>> expression)
        {
            base.AddInclude(expression);
            return this;
        }
        public new GeneralSpecifications<T> AddInclude(List<Expression<Func<T, object>>> expressions)
        {
            base.AddInclude(expressions);
            return this;
        }
    }
}
