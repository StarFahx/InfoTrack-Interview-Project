export default interface PaginationResponse<T> {
    data : T[];
    total : number;
}