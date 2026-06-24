export default interface SolicitorSummary {
    name : string,
    shortDescription : string | null,
    id : string,
    rating : Rating | null
};

export interface Rating {
    value : number,
    maximum : number,
    provider : string
}

export interface Location {
    address : string,
    phone : string,
    ratings : Rating[]
}

export interface SolicitorInfo {
    name : string,
    shortDescription : string | null,
    id : string,
    phone? : string,
    email? : string,
    website? : string,

    ratings : Rating[],
    locations : Location[]
}