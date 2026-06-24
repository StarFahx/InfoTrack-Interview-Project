import ListItemText from "@mui/material/ListItemText";
import SolicitorSummary from "../models/solicitorSummary";
import ListItemButton from "@mui/material/ListItemButton";
import Typography from "@mui/material/Typography";
import Rating from "@mui/material/Rating";
import Divider from "@mui/material/Divider";

type ResultProps = SolicitorSummary & { last : boolean, onClick: () => void };

export default function Result(props : ResultProps) {
    return (
        <>
            <ListItemButton 
                alignItems='flex-start'
                onClick={props.onClick}
            >
                <ListItemText 
                    primary={
                        <>
                            <Typography
                                component='span'
                                variant='h6'
                                sx={{marginRight:'10px'}}
                            >
                                {props.name}
                            </Typography>
                            {props.rating &&
                                <Rating 
                                    readOnly
                                    value={props.rating.value * 5.0 / props.rating.maximum}
                                    precision={0.1}
                                    size='small'
                                    sx={{top: '2px'}}
                                />
                            }
                        </>
                    } 
                    secondary={props.shortDescription}
                />
            </ListItemButton>
            {props.last || <Divider variant='middle' component="li" />}
        </>
    );
}