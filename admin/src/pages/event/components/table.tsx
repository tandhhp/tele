import { apiCheckoutTable, apiGetTableOptions } from "@/services/contact";
import { AuditOutlined } from "@ant-design/icons";
import { Button, Divider, Drawer, message, Popconfirm } from "antd";
import { useEffect, useState } from "react";

type Props = & {
    eventTime: string;
    eventDate: string;
}

const TableComponent: React.FC<Props> = ({ eventDate, eventTime }) => {

    const [open, setOpen] = useState<boolean>(false);
    const [tables1, setTables1] = useState<any[]>([]);

    const fetchTable = () => {
        apiGetTableOptions({
            eventDate, eventTime, floor: 'Lầu 1'
        }).then(response => {
            setTables1(response);
        })
    }

    useEffect(() => {
        if (open) {
            fetchTable();
        }
    }, [open]);

    const onCheckout = async (values: any) => {
        const body = {
            tableId: values.value,
            eventDate,
            eventTime
        }
        await apiCheckoutTable(body);
        message.success('Checkout thành công!');
        fetchTable();
    }

    return (
        <>
            <Button icon={<AuditOutlined />} onClick={() => setOpen(true)}>Bàn</Button>
            <Drawer open={open} onClose={() => setOpen(false)} title="Bàn" width={1000}>
                <div className="flex gap-2 mb-4">
                    <div className="w-4 h-4 bg-green-500"></div> Bàn có sẵn
                    <div className="w-4 h-4 bg-red-500"></div> Bàn bận
                </div>
                <Divider>Danh sách</Divider>
                <div className="grid grid-cols-4 md:grid-cols-8 gap-2">
                    {
                        tables1 && tables1.map((x: any) => (
                            <Popconfirm key={x.value} title="Xác nhận checkout bàn?" onConfirm={() => onCheckout(x)}>
                                <div
                                    className={`cursor-pointer hover:opacity-75 shadow rounded px-1 h-10 flex items-center justify-center text-white font-medium ${x.disabled ? 'bg-red-500' : 'bg-green-500'}`}>
                                    {x.label}
                                </div>
                            </Popconfirm>
                        ))
                    }
                </div>
            </Drawer>
        </>
    )
}

export default TableComponent;