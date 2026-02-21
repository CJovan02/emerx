import {
    AppBar,
    Box,
    IconButton,
    Toolbar,
    Typography,
    Menu,
    MenuItem, Divider, ListItemIcon, ListItemText,
} from "@mui/material";
import {FilterAlt, Logout, Person, ShoppingCart} from "@mui/icons-material";
import {useState} from "react";
import {Spacer} from "../reusable/spacer.tsx";
import * as React from "react";
import StoreTabs from "./storeTabs.tsx";
import {Drawers} from "../../shared/common/constants/drawers.ts";
import useScreenSize from "../../hooks/useScreenSize.tsx";

const drawerWidth = Drawers.Store.Width;

export default function StoreAppBar() {
    const {isDesktop} = useScreenSize();
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleCloseUserMenu = () => {
        setAnchorEl(null);
    };

    return (
        <AppBar
            position={isDesktop ? "fixed" : "static"}
            sx={isDesktop ? {width: `calc(100% - ${drawerWidth}px)`, ml: `${drawerWidth}px`} : {}}
        >
            <Toolbar>

                {/* Drawer Toggle */}
                <IconButton
                    size="large"
                    edge="start"
                    color="inherit"
                    aria-label="open filters drawer"
                >
                    <FilterAlt/>
                </IconButton>

                {/* Logo */}
                <Typography
                    variant="h6"
                    sx={{
                        fontWeight: 700,
                        letterSpacing: ".2rem",
                        mx: 5
                    }}
                >
                    EMERX
                </Typography>

                {/* Navigation Links */}
                <Box sx={{display: "flex", gap: 2}}>
                    <StoreTabs/>
                </Box>

                <Spacer/>

                {/* Avatar Menu */}
                <Box>
                    <IconButton
                        color="inherit"
                        aria-controls={open ? 'account-menu' : undefined}
                        aria-haspopup="true"
                        aria-expanded={open ? 'true' : undefined}
                        onClick={handleOpenUserMenu}
                    >
                        <Person sx={{height: 32, width: 32}}/>
                    </IconButton>

                    <Menu
                        id="account-menu"
                        anchorEl={anchorEl}
                        open={open}
                        onClose={handleCloseUserMenu}
                        anchorOrigin={{
                            vertical: "bottom",
                            horizontal: "right"
                        }}
                        transformOrigin={{
                            vertical: "top",
                            horizontal: "right"
                        }}
                    >
                        <MenuItem onClick={handleCloseUserMenu}>
                            <ListItemIcon>
                                <Person fontSize="small"/>
                            </ListItemIcon>
                            <ListItemText primary='My Profile'/>
                        </MenuItem>
                        <Divider/>
                        <MenuItem onClick={handleCloseUserMenu}>
                            <ListItemIcon>
                                <ShoppingCart fontSize="small"/>
                            </ListItemIcon>
                            <ListItemText primary='Cart'/>
                        </MenuItem>
                        <MenuItem
                            onClick={handleCloseUserMenu}
                            sx={{color: "error.main"}}
                        >
                            <ListItemIcon>
                                <Logout fontSize="small"/>
                            </ListItemIcon>

                            <ListItemText primary="Logout"/>
                        </MenuItem>
                    </Menu>
                </Box>

            </Toolbar>
        </AppBar>
    );
}