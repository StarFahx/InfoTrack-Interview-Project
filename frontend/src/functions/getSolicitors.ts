import SolicitorSummary from "../models/solicitorSummary";
import PaginationResponse from "../models/paginationResponse";
import FilterState from "../models/filterState";
import api from "./axios";

export default async function GetSolicitors(filters : FilterState) {

    let queryString = `pageSize=${filters.resultsPerPage}&pageNumber=${filters.currentPage}`;

    if (filters.namePart != null && filters.namePart !== '') {
        queryString += `&nameFilter=${filters.namePart}`;
    }

    if (!filters.nameOnly) {
        var minRating = filters.minRating;
        if (minRating == null) {
            minRating = 0;
        }
        queryString += `&minRating=${minRating}&ratingsProvider=${filters.ratingsProvider}`;

        if (filters.cities.length > 0) {
            queryString += '&cities=' + filters.cities.join('&cities=');
        }
    }
        
    if (filters.sortBy != null) {
        queryString += `&orderingType=${filters.sortBy}`;
    }

    const response = await api.get(`/solicitors?${queryString}`);
    return response.data as PaginationResponse<SolicitorSummary>;
}