import { render } from 'preact';
import Button from '@mui/material/Button';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useState } from 'preact/compat';
import { createTheme, Theme, ThemeProvider } from '@mui/material/styles';
import DarkMode from '@mui/icons-material/DarkMode';
import LightMode from '@mui/icons-material/LightMode';
import { CssBaseline, Tooltip } from '@mui/material';
import Header, { ThemeWrapper } from './components/header';
import Results from './components/results';

function prefersDarkMode() {
	const darkModeMql = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)');
	return darkModeMql && darkModeMql.matches;
}

const darkTheme = createTheme({
    palette: {
		mode: 'dark',
        primary: {
			light: '#B79AD5',
            main: '#A27EC9',
			dark: '#8D61BD'
        },
        secondary: {
			light: '#70DBFF',
            main: '#47D1FF',
			dark: '#1FC7FF'
        },
		error: {
			light: '#E73936',
            main: '#DD1C1A',
			dark: '#B71815'
		},
		warning: {
			light: '#FA804C',
            main: '#F9611F',
			dark: '#EF4C06'
		},
		info : {
			light: '#FFEB99',
            main: '#FFE066',
			dark: '#FFDA47'
		},
		success: {
			light: '#6ADC98',
            main: '#48D480',
			dark: '#2FC66B'
		},
		background: {
			default: '#1D2535',
			paper: '#242E42'
		}
    }
});

const lightTheme = createTheme({
    palette: {
		mode: 'light',
        primary: {
			light: '#B79AD5',
            main: '#A27EC9',
			dark: '#8D61BD'
        },
        secondary: {
			light: '#70DBFF',
            main: '#47D1FF',
			dark: '#1FC7FF'
        },
		error: {
			light: '#E73936',
            main: '#DD1C1A',
			dark: '#B71815'
		},
		warning: {
			light: '#FA804C',
            main: '#F9611F',
			dark: '#EF4C06'
		},
		info : {
			light: '#FFEB99',
            main: '#FFE066',
			dark: '#FFDA47'
		},
		success: {
			light: '#6ADC98',
            main: '#48D480',
			dark: '#2FC66B'
		},
		background: {
			default: '#EFFFFF',
			paper: '#e8f1f1'
		}
    }
});

export function App() {
	const [value, setValue] = useState(0);
	const [theme, setTheme] = useState<ThemeWrapper>({
		theme : prefersDarkMode() ? darkTheme : lightTheme,
		value : prefersDarkMode() ? 'dark' : 'light'
	});

	const handleChange = (event: any, newValue: number) => {
		setValue(newValue);
	};

	const toggleTheme = () => {
		setTheme(theme.value == 'dark' 
			? { theme : lightTheme, value : 'light' }
			: { theme : darkTheme, value : 'dark' });
	}

	return (
		<ThemeProvider theme={theme.theme}>
      		<CssBaseline />
			<Header theme={theme} toggleTheme={toggleTheme} />
			<Container maxWidth="lg" sx={{marginTop: '10px'}}>
				<Results />
			</Container>
		</ThemeProvider>
	);
}

render(<App />, document.getElementById('app'));
