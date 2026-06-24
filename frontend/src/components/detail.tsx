import Box from "@mui/material/Box";
import Modal from "@mui/material/Modal";
import { forwardRef, useEffect, useState } from "react";
import { SolicitorInfo } from "../models/solicitorSummary";
import getSolicitor from "../functions/getSolicitor";
import Card from "@mui/material/Card";
import { Grid, Rating, Typography } from "@mui/material";
import capitaliseFirstLetter from "../functions/capitaliseFirstLetter";

interface ClosedProps {
    open : false;
}

interface OpenProps {
    open : true;
    id : string;
}

export type DetailInfo = OpenProps | ClosedProps;
type DetailProps = DetailInfo & { onClose : (error? : string) => void };


export default function Detail(props : DetailProps) {
    const [data, setData] = useState<SolicitorInfo | 'loading'>('loading');

    const handleResult = (result : SolicitorInfo | undefined) => {
        if (data == 'loading' && props.open && props.id == result?.id) {
            setData(result);
        } else if (!result) {
            handleClose('An error occurred loading the solicitor');
        }
    }

    const handleClose = (error? : string) => {
        props.onClose(error);
        setData('loading');
    }

    useEffect(() => {
        if (props.open && data == 'loading') {
            getSolicitor(props.id)
                .then(result => handleResult(result));
        }
    }, [props.open]);

    useEffect(() => {}, [data])
    
    return (
        <Modal
            open={props.open}
            onClose={() => handleClose()}
        >
            <DetailDisplay data={data} />
            
        </Modal>
    );
}

const style = {
  position: 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 800,
  maxWidth: '60%',
  maxHeight: '80%',
  bgcolor: 'background.default',
  overflow: 'scroll',
  p: 4,
};

const DetailDisplay = forwardRef((props : {data : SolicitorInfo | 'loading'}) => {
    if (props.data === 'loading') {
        return (
            <Box sx={style}>
                Waiting...
            </Box>
        );
    } else {
        return (
            <Box sx={style}>
                <Typography variant='h3' component='h2'>{props.data.name}</Typography>
                <Typography variant='body1'>{props.data.shortDescription}</Typography>
                {props.data.ratings &&
                    (<>
                        <br />
                        <Typography variant='h5' component='h3'>Ratings</Typography>
                        <Grid container direction='row' spacing={2} sx={{justifyContent: 'flex-start', alignItems: 'stretch'}}>
                        {props.data.ratings.map(rating => (
                            <Grid size={3}>
                                <Box sx={{padding: '15px', height: '100%'}}>
                                    <Typography variant='body2'>{capitaliseFirstLetter(rating.provider)}</Typography>
                                    <Rating 
                                        readOnly
                                        value={rating.value * 5.0 / rating.maximum}
                                        precision={0.1}
                                        size='small'
                                        sx={{top: '2px'}}
                                    />
                                </Box>
                            </Grid>
                        ))}
                        </Grid>
                    </>)
                }
                {props.data.locations &&
                    (<>
                        <br />
                        <Typography variant='h5' component='h3'>Offices</Typography>
                        <Grid container direction='row' spacing={2} sx={{justifyContent: 'flex-start', alignItems: 'stretch'}}>
                        {props.data.locations.map(office => (
                            <Grid size={3}>
                                <Card sx={{padding: '15px', height: '100%'}}>
                                    {office.address.split('\n').map(line => (<Typography variant='body2'>{line}</Typography>))}
                                    {office.phone && <><br/><Typography variant='body2'>tel: {office.phone}</Typography></>}
                                    {office.ratings && office.ratings.map(rating => 
                                        (<>
                                            <Typography variant='body2'>{capitaliseFirstLetter(rating.provider)}</Typography>
                                            <Rating 
                                                readOnly
                                                value={rating.value * 5.0 / rating.maximum}
                                                precision={0.1}
                                                size='small'
                                                sx={{top: '2px'}}
                                            />
                                        </>)
                                    )}
                                </Card>
                            </Grid>
                        ))}
                        </Grid>
                    </>)
                }
                
            </Box>
        );
    }
});