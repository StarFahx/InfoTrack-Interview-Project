export default interface FilterState {
    cities : string[];
    minRating : number;
    resultsPerPage : number;
    currentPage : number;
    ratingsProvider : string;
    sortBy : string;
    namePart : string;
    nameOnly : boolean;
};