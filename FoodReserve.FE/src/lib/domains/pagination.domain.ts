export interface Pagination<T> {
    response: T[];
    current_page: number;
    total_pages: number;
    total_count: number;
}