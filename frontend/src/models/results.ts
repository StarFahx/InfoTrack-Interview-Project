import PaginationResponse from "./paginationResponse";
import SolicitorSummary from "./solicitorSummary";

type Results = SuccessResults | ErrorResults | WaitingResults;

interface SuccessResults {
    type : 'Success',
    data : PaginationResponse<SolicitorSummary>
};

interface ErrorResults {
    type : 'Error' | 'Warning',
    message : string
};

interface WaitingResults {
    type : 'Waiting'
};

const waitingResults : WaitingResults = {
    type : 'Waiting'
};

export default Results;
export {waitingResults};