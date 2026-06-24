import List from "@mui/material/List";
import Pagination from "@mui/material/Pagination";
import FilterState from "../models/filterState";
import Results from "../models/results";
import Result from "./result";
import Detail, { DetailInfo } from "./detail";
import { useState } from "react";

interface ResultsListProps {
    results : Results;
    filters : FilterState;
    setFilters : (state : FilterState) => void;
}

export default function ResultsList(props : ResultsListProps) {
    const [detail, setDetail] = useState<DetailInfo>({open: false});

    switch (props.results.type) {
        case 'Success':
            let data = props.results.data.data;
            return (
                <>
                    <List sx={{ width: '100%', maxHeight: '85vh', overflow: 'scroll' }}>
                        {data.map((r, i) => (<Result {...r} onClick={() => setDetail({open : true, id : r.id})} last={i == data.length - 1} />))}
                    </List>
                    <Detail {...detail} onClose={() => setDetail({open: false})} />
                    <Pagination 
                        page={props.filters.currentPage}
                        count={Math.ceil(props.results.data.total / props.filters.resultsPerPage)} 
                        shape='rounded'
                        onChange={(_, page) => props.setFilters(Object.assign({}, {...props.filters}, {currentPage: page}))}
                    />
                </>
            );
        case 'Waiting':
            return <div>Waiting...</div>;
        case 'Error':
            return <div>Error...</div>;
        case 'Warning':
            return <div>Warning...</div>;
    }
}