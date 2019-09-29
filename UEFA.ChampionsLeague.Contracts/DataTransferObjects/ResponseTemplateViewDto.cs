namespace UEFA.ChampionsLeague.Contracts.DataTransferObjects
{
    public class ResponseTemplateViewDto<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
    }
}
