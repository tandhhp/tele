import { apiKeyInStatusOptions } from "@/services/contact";
import { apiDebtHistoryList, apiListKeyInRevenue, apiTopupKeyIn } from "@/services/event";
import { ClockCircleOutlined, DeleteOutlined, MoneyCollectOutlined, MoreOutlined, SettingOutlined, UploadOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProForm, ProFormDigit, ProFormInstance, ProFormText, ProFormTextArea, ProTable } from "@ant-design/pro-components";
import { Button, Drawer, Dropdown, message, Tag, Image } from "antd";
import { useEffect, useRef, useState } from "react";

const Index: React.FC = () => {

    const [openHistory, setOpenHistory] = useState(false);
    const [keyIn, setKeyIn] = useState<any>(null);
    const historyActionRef = useRef<ActionType>(null);
    const [openTopup, setOpenTopup] = useState(false);
    const actionRef = useRef<ActionType>(null);
    const formRef = useRef<ProFormInstance>(null);
    const [fileList, setFileList] = useState<any>([]);

    useEffect(() => {
        if (keyIn) {
            historyActionRef.current?.reload();
        }
    }, [keyIn]);

    useEffect(() => {
        formRef.current?.setFields([
            {
                name: 'contractCode',
                value: keyIn?.contractCode
            }
        ])
    }, [keyIn]);

    const onTopup = async (values: any) => {
        values.leadId = keyIn?.keyInId;
        const formData = new FormData();
        formData.append('contractCode', values.contractCode);
        formData.append('amount', values.amount);
        formData.append('note', values.note);
        fileList.forEach((file: any) => {
            formData.append('evidences', file);
        });
        formData.append('leadId', values.leadId);
        await apiTopupKeyIn(formData);
        message.success('Nạp tiền thành công');
        setOpenTopup(false);
        actionRef.current?.reload();
        formRef.current?.resetFields();
        setFileList([]);
    }

    return (
        <PageContainer>
            <ProTable
                actionRef={actionRef}
                scroll={{ x: true }}
                request={apiListKeyInRevenue}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30
                    },
                    {
                        title: 'Mã HĐ',
                        dataIndex: 'contractCode',
                        width: 100,
                        search: false
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'keyInName',
                        minWidth: 100
                    },
                    {
                        title: 'Điện thoại',
                        dataIndex: 'keyInPhoneNumber',
                        width: 120
                    },
                    {
                        title: 'Ngày sự kiện',
                        dataIndex: 'eventDate',
                        valueType: 'date',
                        width: 100,
                        search: false
                    },
                    {
                        title: 'Check-In',
                        dataIndex: 'eventTime',
                        width: 80,
                        search: false
                    },
                    {
                        title: 'Trợ lý cá nhân',
                        dataIndex: 'saleName'
                    },
                    {
                        title: 'Tổng tiền',
                        dataIndex: 'amount',
                        valueType: 'digit',
                        width: 120,
                        render: (text) => <div>{text} ₫</div>,
                        search: false
                    },
                    {
                        title: 'Chờ duyệt',
                        dataIndex: 'amountPending',
                        valueType: 'digit',
                        width: 120,
                        render: (text) => <div>{text} ₫</div>,
                        search: false
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                        valueType: 'select',
                        request: apiKeyInStatusOptions,
                        search: false
                    },
                    {
                        title: <SettingOutlined />,
                        valueType: 'option',
                        width: 50,
                        render: (text, record) => [
                            <Dropdown key="more" menu={{
                                items: [
                                    {
                                        key: 'top-up',
                                        label: 'Nạp tiền',
                                        onClick: () => {
                                            setKeyIn(record);
                                            setOpenTopup(true);
                                        },
                                        icon: <MoneyCollectOutlined />
                                    },
                                    {
                                        key: 'history',
                                        label: 'Lịch sử',
                                        onClick: () => {
                                            setKeyIn(record);
                                            setOpenHistory(true);
                                        },
                                        icon: <ClockCircleOutlined />
                                    },
                                ]
                            }}>
                                <Button type="dashed" size="small" icon={<MoreOutlined />} />
                            </Dropdown>
                        ]
                    }
                ]}
                search={{
                    layout: 'vertical'
                }}
            />
            <ModalForm open={openTopup} onOpenChange={setOpenTopup} title={`Nạp tiền cho ${keyIn?.keyInName}`} onFinish={onTopup} formRef={formRef}>
                <div className="flex gap-4">
                    <div className="w-40">
                        <ProFormText name="contractCode" label="Mã HĐ" rules={[{ required: true }]} />
                    </div>
                    <div className="flex-1">
                        <ProFormDigit name="amount" label="Số tiền" rules={[{ required: true }]} fieldProps={{
                            suffix: '₫',
                            formatter: (value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                            parser: (value) => value?.replace(/(,*)/g, '') as unknown as number
                        }} />
                    </div>
                </div>
                <ProFormTextArea name="note" label="Ghi chú" />
                <ProForm.Item label="Minh chứng" name="evidence">
                    <div className="flex gap-4 flex-wrap">
                        {
                            fileList.map((file: any, index: number) => {
                                const item = URL.createObjectURL(file);
                                return (
                                    <div key={index} className="relative">
                                        <Image src={item} className="w-20 h-20 object-contain bg-slate-100" width={80} height={80} />
                                        <Button className="absolute top-0 right-0" type="primary" danger icon={<DeleteOutlined />} size="small" onClick={() => {
                                            const newList = [...fileList];
                                            newList.splice(index, 1);
                                            setFileList(newList);
                                        }}>
                                        </Button>
                                    </div>
                                )
                            })
                        }
                        <div className="flex items-center jusitfy-center w-20 h-20 relative hover:border-blue-500 border bg-slate-100 rounded cursor-pointer">
                            <input type="file" accept="image/*" className="absolute opacity-0 inset-0 w-full h-full" onChange={(e) => {
                                const files = e.target.files;
                                if (files) {
                                    const newFiles = Array.from(files);
                                    setFileList([...fileList, ...newFiles]);
                                }
                            }} />
                            <div className="flex justify-center w-full"><UploadOutlined className="text-2xl text-slate-500" /></div>
                        </div>
                    </div>
                </ProForm.Item>
            </ModalForm>
            <Drawer
                title="Lịch sử"
                placement="right"
                onClose={() => setOpenHistory(false)}
                open={openHistory}
                width={1000}>
                <ProTable
                    ghost
                    actionRef={historyActionRef}
                    request={(params) => apiDebtHistoryList({ ...params, keyInId: keyIn?.keyInId })}
                    rowKey="id"
                    search={false}
                    columns={[
                        {
                            title: '#',
                            valueType: 'indexBorder',
                            width: 30
                        },
                        {
                            title: 'Mã HĐ',
                            dataIndex: 'contractCode',
                            width: 100
                        },
                        {
                            title: 'Thời gian',
                            dataIndex: 'createdDate',
                            valueType: 'date',
                            width: 120
                        },
                        {
                            title: 'Số tiền',
                            dataIndex: 'amount',
                            valueType: 'digit',
                        },
                        {
                            title: 'Người tạo',
                            dataIndex: 'creator',
                        },
                        {
                            title: 'Minh chứng',
                            dataIndex: 'evidences',
                            render: (dom, entity) => {
                                return (
                                    <div className="flex gap-2">
                                        {
                                            entity.evidences.map((item: any, index: number) => {
                                                return (
                                                    <Image key={index} src={item} className="w-14 h-14 object-contain bg-slate-100" width={52} height={52} />
                                                )
                                            })
                                        }
                                    </div>
                                )
                            }
                        },
                        {
                            title: 'Trạng thái',
                            dataIndex: 'status',
                            valueType: 'select',
                            width: 100,
                            valueEnum: {
                                0: { text: <Tag color="warning" className="w-full text-center">Chờ duyệt</Tag> },
                                1: { text: <Tag color="blue" className="w-full text-center">GD duyệt</Tag> },
                                2: { text: <Tag color="success" className="w-full text-center">KT duyệt</Tag> },
                                3: { text: <Tag color="error" className="w-full text-center">Từ chối</Tag> },
                            },
                        },
                        {
                            title: 'Ghi chú',
                            dataIndex: 'note',
                        }
                    ]}
                />
            </Drawer>
        </PageContainer>
    );
}

export default Index;