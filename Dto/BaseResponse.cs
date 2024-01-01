namespace SimpleAPI.Dto
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public int Code { get; set; }
        public ErrorResponse Error { get; set; }
    }
}
