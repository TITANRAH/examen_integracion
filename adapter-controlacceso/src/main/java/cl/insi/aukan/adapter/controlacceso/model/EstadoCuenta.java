package cl.insi.aukan.adapter.controlacceso.model;

/**
 * Modelo del mensaje de estado de cuenta (JSON)
 */
public class EstadoCuenta {
    private EstadoCuentaData estadoCuenta;

    public EstadoCuentaData getEstadoCuenta() {
        return estadoCuenta;
    }

    public void setEstadoCuenta(EstadoCuentaData estadoCuenta) {
        this.estadoCuenta = estadoCuenta;
    }

    public static class EstadoCuentaData {
        private Cliente cliente;
        private Cuenta cuenta;

        public Cliente getCliente() {
            return cliente;
        }

        public void setCliente(Cliente cliente) {
            this.cliente = cliente;
        }

        public Cuenta getCuenta() {
            return cuenta;
        }

        public void setCuenta(Cuenta cuenta) {
            this.cuenta = cuenta;
        }
    }

    public static class Cliente {
        private String rut;

        public String getRut() {
            return rut;
        }

        public void setRut(String rut) {
            this.rut = rut;
        }
    }

    public static class Cuenta {
        private int saldo;
        private String estado;

        public int getSaldo() {
            return saldo;
        }

        public void setSaldo(int saldo) {
            this.saldo = saldo;
        }

        public String getEstado() {
            return estado;
        }

        public void setEstado(String estado) {
            this.estado = estado;
        }
    }
}
