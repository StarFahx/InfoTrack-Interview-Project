import { Autocomplete, Box, Divider, Grid, InputLabel, MenuItem, Paper, Rating, Select, SelectChangeEvent, Switch, TextField, ToggleButton, Typography } from "@mui/material";
import getCities from "../functions/getCities";
import FilterState from "../models/filterState";
import { useEffect, useState } from "react";
import NumberField from "./numberField";
import getRatingsProviders from "../functions/getRatingsProviders";
import capitaliseFirstLetter from "../functions/capitaliseFirstLetter";

interface SidebarProps {
    filters : FilterState;
    onChange : (filters : FilterState) => void;
}

export default function Sidebar(props : SidebarProps) {
    const [cities, setCities] = useState<string[]>([]);
    const [ratingsProviders, setRatingsProviders] = useState<string[]>(['Solicitors.com']);

    useEffect(() => {
        if (cities.length === 0) {
            getCities().then((results) => setCities(results));
        }
    }, [cities.length]);

    useEffect(() => {
        if (cities.length === 0) {
            getRatingsProviders().then((results) => setRatingsProviders(results));
        }
    }, [cities.length]);
    
    return (
        <Paper sx={{margin: '8px', padding: '8px'}}>
            <Box sx={{p: 2}}>
                <Typography variant='h5' component='h2'>Filter Conveyancers</Typography>
            </Box>
            <Box sx={{ p: 2}}>
                <Typography component='legend'>Firm Name</Typography>
                <TextField
                    label='Firm Name'
                    variant='standard'
                    sx={{width: '100%'}}
                    value={props.filters.namePart}
                    onChange={e => props.onChange(Object.assign({}, {...props.filters}, {namePart: (e.target as any).value, currentPage: 1}))}
                />
            </Box>
            <Divider />
            <Box sx={{ p: 2}}>
                <Typography component='legend'>Search by Firm Name Only</Typography>
                <Switch
                    value={props.filters.nameOnly}
                    onChange={(_, value) => props.onChange(Object.assign({}, {...props.filters}, {nameOnly: value, currentPage: 1}))}
                />
            </Box>
            <Divider />
            {!props.filters.nameOnly && 
            <>
                <Box sx={{ p: 2}}>
                    <Typography component='legend'>Cities</Typography>
                    <Autocomplete
                        multiple
                        options={cities}
                        value={props.filters.cities}
                        getOptionLabel={capitaliseFirstLetter}
                        onChange={(_, value) => props.onChange(Object.assign({}, {...props.filters}, {cities: value, currentPage: 1}))}
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="standard"
                                label="Cities"
                                placeholder=""
                            />
                        )}
                    />
                </Box>
                <Divider />
                <Box sx={{p: 2}}>
                    <Grid container spacing={1} sx={{width: '100%'}}>
                        <Grid size={6}>
                            <Typography component='legend'>Min. Rating</Typography>
                            <Rating
                                name='Min. Rating'
                                precision={0.5}
                                value={props.filters.minRating}
                                size='medium'  
                                onChange={(_, value) => props.onChange(Object.assign({}, {...props.filters}, {minRating: value, currentPage: 1}))}
                            />
                        </Grid>
                        <Grid size={6}>
                            <InputLabel id='ratings-label'>Rating Provider</InputLabel>
                            <Select
                                labelId='ratings-label'
                                value={props.filters.ratingsProvider}
                                onChange={(e : SelectChangeEvent) => props.onChange(Object.assign({}, {...props.filters}, {ratingsProvider: (e.target as any).value, currentPage: 1}))}
                            >
                                {ratingsProviders.map(provider => {
                                    return (
                                        <MenuItem 
                                            value={provider} 
                                            selected={provider === props.filters.ratingsProvider}>{capitaliseFirstLetter(provider)}
                                        </MenuItem>
                                    )
                                })}
                            </Select>
                        </Grid>
                    </Grid>
                </Box>
                <Divider />
            </>}
            <Box sx={{p: 2}}>
                <Typography component='legend'>Results per Page</Typography>
                <NumberField 
                    label="" 
                    value={props.filters.resultsPerPage}
                    onValueChange={value => props.onChange(Object.assign({}, {...props.filters}, {resultsPerPage: value, currentPage: 1}))}
                    min={10} 
                    max={40} 
                />
            </Box>
            <Divider />
            <Box sx={{p: 2}}>
                <InputLabel id='sort-label'>Sort By</InputLabel>
                <Select
                    labelId='sort-label'
                    value={props.filters.sortBy}
                    onChange={(e : SelectChangeEvent) => props.onChange(Object.assign({}, {...props.filters}, {sortBy: (e.target as any).value, currentPage: 1}))}
                >
                    {sortingOptions.map(option => {
                        return (
                            <MenuItem 
                                value={option.value} 
                                selected={option.value === props.filters.sortBy}
                            >
                                {option.name}
                            </MenuItem>
                        )
                    })}
                </Select>
            </Box>
        </Paper>
    )
}

const sortingOptions = [
    {
        value: 'RatingDescending',
        name: 'Rating (descending)'
    },
    {
        value: 'RatingAscending',
        name: 'Rating (ascending)'
    },
    {
        value: 'AlphabetAscending',
        name: 'Alphabetical (ascending)'
    },
    {
        value: 'AlphabetDescending',
        name: 'Alphabetical (descending)'
    }
]