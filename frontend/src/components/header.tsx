import DarkMode from "@mui/icons-material/DarkMode";
import LightMode from "@mui/icons-material/LightMode";
import { AppBar, IconButton, Toolbar, Tooltip, Typography } from "@mui/material";
import { Theme } from "@mui/material/styles";

export interface ThemeWrapper {
    theme : Theme,
    value : 'dark' | 'light';
}

interface HeaderProps {
    theme : ThemeWrapper;
    toggleTheme : () => void;
}

export default function Header(props : HeaderProps) {
    return (
        <AppBar position='static' sx={{}}>
            <Toolbar sx={{bgcolor:'primary.main'}}>
                <Typography variant="h4" component="h1" sx={{ flexGrow: 2, marginLeft: '16px' }}>
                    Conveyancing Search
                </Typography>
                <Tooltip title="Toggle theme">
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="theme toggle"
                        sx={{ mr: 2 }}
                        onClick={props.toggleTheme}
                    >
                        {
                            props.theme.value === 'dark'
                                ? <DarkMode />
                                : <LightMode />
                        }
                    </IconButton>
                </Tooltip>
            </Toolbar>
        </AppBar>
    );
}