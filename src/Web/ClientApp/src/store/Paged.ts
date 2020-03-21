export interface Paged<T> {
    rows: T[];
    pageSize: number;
    page: number;
    totalRows: number;
    lastId?: number;
}
