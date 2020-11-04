namespace LeeraJenkins.Model.ApiModel.Base
{
    public abstract class BaseApiResponse
    {
        public bool IsSuccess { get; set; }
    }

    public class ApiResponse<T> : BaseApiResponse
    {
        public T Response { get; set; }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse<T> Ok<T>(T response)
        {
            return new ApiResponse<T>
            {
                Response = response,
                IsSuccess = true
            };
        }
    }
}
