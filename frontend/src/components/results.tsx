import { useEffect, useState } from "preact/hooks";
import GetSolicitors from "../functions/getSolicitors";
import Grid from "@mui/material/Grid";
import Sidebar from "./sidebar";
import FilterState from "../models/filterState";
import { default as ResultsType, waitingResults } from "../models/results";
import ResultsList from "./resultsList";

export default function Results() {
    const [results, setResults] = useState<ResultsType>(waitingResults);
    const [filters, setFilters] = useState<FilterState>({
        cities: [], 
        minRating: 3, 
        resultsPerPage: 20,
        currentPage: 1,
        ratingsProvider: 'Solicitors.com',
        sortBy: 'RatingDescending',
        namePart: '',
        nameOnly: false});
        
    useEffect(() => {
        if (results.type == 'Waiting') {
            GetSolicitors(filters)
                .then(result => setResults({type: 'Success', data: result}));
        }
    }, [results]);

    useEffect(() => {
        setResults({type: 'Waiting'});
    }, [filters]);

    return (
        <Grid container spacing={0} sx={{minHeight: '100vh'}}>
            <Grid size={4}>
                <Sidebar
                    filters={filters}
                    onChange={setFilters}
                />
            </Grid>
            <Grid size={8}>
                <ResultsList results={results} filters={filters} setFilters={setFilters} />
            </Grid>
        </Grid>
    );
}

